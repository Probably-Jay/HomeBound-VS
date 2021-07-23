using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rythm
{

    public class MusicManager : MonoBehaviour
    {
        private void OnEnable()
        {
            multiAudioSource.OnChangedMusic += InvokeSongChanged;
        }

        private void OnDisable()
        {
            multiAudioSource.OnChangedMusic -= InvokeSongChanged;
        }

        private void InvokeSongChanged(RythmSong song) => OnChangedMusic?.Invoke(song);

        private AudioSource CurrentAudioSource => multiAudioSource.CurrentSongSource;
        private RythmSong CurrentSong => multiAudioSource.CurrentSong;

        // private AudioSource
        [SerializeField] RythmSong defaultMusic;
        [SerializeField] MultiAudioSource multiAudioSource = new MultiAudioSource();
        [SerializeField] private bool playOnStart;

        public event Action<RythmSong> OnChangedMusic;

        public bool PlayingMusic => CurrentAudioSource == null ? false :  CurrentAudioSource.isPlaying;
        public int CurrentSample => CurrentAudioSource.timeSamples;
        public int CurrentClipFrequency => CurrentClip.frequency;
        public int CurrentClipTotalSamples => CurrentClip.samples;
        public float CurrentSongBPM => CurrentSong.BPM;
        public float CurrentSongSampleOffset => CurrentSong.beginAtSample;
        public int CurrentSongPickupBeats => CurrentSong.pickupBeats;
        private AudioClip CurrentClip => CurrentAudioSource.clip;


        private void Awake()
        {
            multiAudioSource.Init(this, GetComponent<AudioSource>());
        }

        private void Start()
        {
            if (playOnStart)
            {
                multiAudioSource.InitFirstClip(defaultMusic, defaultMusic.beginAtSample); 
                //RythmEngine.Instance.SetTrackInfo(defaultMusic);
            }
        }

        /// <summary>
        /// Play provided <paramref name="music"/> and add it to the song-stack. Pause the current song so that it can be returned to later
        /// </summary>
        public void PushNewSong(RythmSong music)
        {
            multiAudioSource.PushNewClip(music, music.beginAtSample);
        }        
        
        public void PushNewSong(RythmSong music, int beginAtSample)
        {
            multiAudioSource.PushNewClip(music, beginAtSample);
        }



        /// <summary>
        /// Play provided <paramref name="music"/> and stop the current song
        /// </summary>
        public void MutateCurrentSongToNewSong(RythmSong music)
        {
            multiAudioSource.MutateCurrentClip(music, music.beginAtSample);
        }

        /// <summary>
        /// Return to the song playing before the last call of <see cref="PushNewSong(RythmSong)"/>
        /// </summary>
        public void ReturnToPreviousSong()
        {
            multiAudioSource.ReturnToPreviousClip();
        }

 

        /// <summary>
        /// Pauses the top song
        /// </summary>
        public void PauseSinlgeSong()
        {
            multiAudioSource.PauseClip();
        }

        public void ResumeSingleSong()
        {
            multiAudioSource.ResumeClip();
        }
   
        //public void SetSongTime(int sample)
        //{
        //    CurrentAudioSource.timeSamples = sample;
        //}

        /// <summary>
        /// Play provided <paramref name="music"/> and add it to the song-stack, plays the backing and starts the melody paused. Pause the current song so that it can be returned to later
        /// </summary>
        public void StartRhythmSection(SplitRythmSong music)
        {
            multiAudioSource.PushNewSplitClip(music, music.backingRhythmSong.beginAtSample, music.melodySong.beginAtSample);
        }

        public void PauseMelody()
        {
            multiAudioSource.PauseClipMelodyClip();
        }

        public void ResumeMelody()
        {
            multiAudioSource.ResumeMelodyClip();
        }

        public void StopRythmSection()
        {
            multiAudioSource.EndSplitClip();
        }

        internal void SyncSongTime(int sample, int difference)
        {
            float a = RythmEngine.Instance.SamplesToSeconds(difference);
            Debug.LogWarning($"Re-syncing music. Jumping {difference} samples ({a}s)");
            if(Mathf.Abs(difference) < 400)
            {
                int b = 0;
                b++;
            }
            multiAudioSource.SyncSongTime(sample);
        }




        [System.Serializable]
        private class MultiAudioSource
        {
            [SerializeField] private AnimationCurve fadeInMusicCurve;
            [SerializeField] private AnimationCurve fadeOutMusicCurve;
            public AudioSource CurrentSongSource => !PlayingSeperateMelody ? CurrentTop?.AudioSource : CurrentMelody.AudioSource;
            public RythmSong CurrentSong => !PlayingSeperateMelody ? CurrentTop?.RythmSong : CurrentMelody.RythmSong;

            public event Action<RythmSong> OnChangedMusic;

            private SourceAndSong CurrentTop
            {
                get
                {
                    if (activeSources.Count == 0)
                    {
                        Debug.LogWarning($"No music is playing, {nameof(activeSources)} is empty.");
                        return null;
                    }
                    
                    return activeSources.Peek();   
                }
            }

            private SourceAndSong CurrentMelody 
            {
                get
                {
                    if(mode != Mode.RhythmSection)
                    {
                        throw new Exception("There is no seperate melody as you are not in a rhythm section");
                    }

                    return melody;
                }
                set
                {
                    melody = value;
                }
            }

            private bool PlayingSeperateMelody => mode == Mode.RhythmSection;

            private Stack<SourceAndSong> activeSources = new Stack<SourceAndSong>();
            private Queue<SourceAndSong> inactiveSources = new Queue<SourceAndSong>();
            private SourceAndSong melody = null;
            private MonoBehaviour parentBehaviour;

            enum Mode 
            { 
                SinlgeTrackSong
                ,RhythmSection
            }

            
            private Mode mode = Mode.SinlgeTrackSong;

            internal void Init(MonoBehaviour parentBehaviour, AudioSource audioSource)
            {
                if(audioSource == null)
                {
                    throw new Exception("Must have a audiosources");
                }
                this.parentBehaviour = parentBehaviour;
                inactiveSources.Enqueue(new SourceAndSong(audioSource, fadeInMusicCurve, fadeOutMusicCurve, parentBehaviour));
            }

            /// <summary>Same as <see cref="PushNewSong(RythmSong,int)"/> except will not warn that a song is not currently in the stack</summary>
            public RythmSong InitFirstClip(RythmSong newSong, int fromSample = 0)
            {
                var newSongAndSource = GetUnusedAudioSource();

                SetNewSong(newSongAndSource, newSong, fromSample);

                parentBehaviour.StartCoroutine(PlayNewSong(newSongAndSource));

                return CurrentSong;
            }

            public RythmSong PushNewClip(RythmSong newSong, int fromSample = 0, bool play = true )
            {
                if (mode != Mode.SinlgeTrackSong)
                {
                    throw new Exception("Cannot begin rhythm section when already in one");
                }

                var newSongAndSource = GetUnusedAudioSource();

                var oldSource = CurrentTop;

                SetNewSong(newSongAndSource, newSong, fromSample);

                if(play)
                    parentBehaviour.StartCoroutine(PlayNewSong(newSongAndSource));

                if(oldSource != null)
                    parentBehaviour.StartCoroutine(PauseSong(oldSource));

                return CurrentSong;
            }


            public RythmSong MutateCurrentClip(RythmSong newSong, int fromSample = 0)
            { 
                SourceAndSong newSongAndSource = GetUnusedAudioSource();
                var oldSource = SetPreviousSong();

                SetNewSong(newSongAndSource, newSong, fromSample);

                parentBehaviour.StartCoroutine(PlayNewSong(newSongAndSource));
                parentBehaviour.StartCoroutine(StopAndKillSource(oldSource));

                return CurrentSong;
            }

          

            public RythmSong ReturnToPreviousClip(bool resumePrevious = true)
            {
                if (!(activeSources.Count > 1))
                {
                    Debug.LogError("No past song to return to!");
                    return CurrentSong;
                }

                SourceAndSong oldSource = SetPreviousSong();
                var newTopSongAndSource = CurrentTop;

                if(resumePrevious)
                    parentBehaviour.StartCoroutine(ResumeSong(newTopSongAndSource));

                parentBehaviour.StartCoroutine(StopAndKillSource(oldSource));

                return CurrentSong;
            }

            public void PauseClip() => parentBehaviour.StartCoroutine(PauseSong(CurrentTop));
            public void PauseClipMelodyClip()
            {
                CurrentMelody.InstantPause();//parentBehaviour.StartCoroutine(PauseSong(melody));
            }

            public void ResumeClip() => parentBehaviour.StartCoroutine(ResumeSong(CurrentTop));
            internal void ResumeMelodyClip()
            {
                CurrentMelody.InstantPlay();//parentBehaviour.StartCoroutine(ResumeSong(melody));
            }

            ///<summary>Set current top to <paramref name="songAndSource"/></summary>
            private void SetNewSong(SourceAndSong songAndSource, RythmSong newSong, int fromSample)
            {
                songAndSource.Set(newSong, fromSample);
                activeSources.Push(songAndSource);
                InvokeSongChange();
            }
            ///<summary>Set current top to element below it in the stack and return poped value</summary>
            private SourceAndSong SetPreviousSong()
            {
                var popped = activeSources.Pop();
                InvokeSongChange();
                return popped;
            }

            private void InvokeSongChange()
            {
                OnChangedMusic?.Invoke(CurrentSong);
            }

            public void PushNewSplitClip(SplitRythmSong music, int beginAtSample, int melodyBeginAtSample)
            {
                PushNewClip(music.melodySong, beginAtSample, play: false);
                CurrentMelody = CurrentTop;
                PushNewClip(music.backingRhythmSong, melodyBeginAtSample);
                mode = Mode.RhythmSection;
            }

            public void EndSplitClip()
            {
                if(mode != Mode.RhythmSection)
                {
                    throw new Exception("Cannot end rhythm section because we are not in one!");
                }

                ReturnToPreviousClip(resumePrevious: false);
                ReturnToPreviousClip();
                RythmEngine.Instance.ReSync();

            }

            private SourceAndSong GetUnusedAudioSource()
            {
                if (inactiveSources.Count == 0)
                {
                    AddNewAudioSource();
                }
                return inactiveSources.Dequeue();
            }
            private void AddNewAudioSource()
            {
                var newSource = parentBehaviour.gameObject.AddComponent<AudioSource>();
                newSource.playOnAwake = false;
                var sourceAndSong = new SourceAndSong(newSource, fadeInMusicCurve, fadeOutMusicCurve, parentBehaviour);
                inactiveSources.Enqueue(sourceAndSong);
            }

            private IEnumerator PlayNewSong(SourceAndSong songAndSource)
            {
                yield return songAndSource.FadeInPlay();
            }

            private IEnumerator ResumeSong(SourceAndSong songAndSource)
            {
                yield return songAndSource.FadeInPlay();
            }

            private IEnumerator PauseSong(SourceAndSong oldSource)
            {
                yield return oldSource.FadeOutPause();
            }

            private IEnumerator StopAndKillSource(SourceAndSong oldSource)
            {
                yield return oldSource.FadeOutStopAndUnset();
                inactiveSources.Enqueue(oldSource);
            }

            internal void SyncSongTime(int sample)
            {
                var diff = sample - CurrentSongSource.timeSamples;
                CurrentTop.AudioSource.timeSamples += diff;
                if(melody != null)
                {
                    melody.AudioSource.timeSamples += diff;
                }
            }

            private class SourceAndSong
            {
                private readonly AnimationCurve fadeInCurve;
                private readonly AnimationCurve fadeOutCurve;
                private readonly MonoBehaviour parentBehivaiour;
                private bool playing => AudioSource.isPlaying;

                public SourceAndSong(AudioSource audioSource, AnimationCurve fadeInCurve, AnimationCurve fadeOutCurve, MonoBehaviour parentBehivaiour)
                {
                    this.AudioSource = audioSource;
                    this.fadeInCurve = fadeInCurve;
                    this.fadeOutCurve = fadeOutCurve;
                    this.parentBehivaiour = parentBehivaiour;
                }

                public AudioSource AudioSource { get; private set; }

                public RythmSong RythmSong { get; private set; }

               // public RythmSong RythmSong => rythmSong;

                public void Set(RythmSong song, int fromSample)
                {
                    RythmSong = song;
                    AudioSource.timeSamples = fromSample;
                    AudioSource.clip = song.audioClip;
                }

                public void Unset()
                {
                    RythmSong = null;
                    AudioSource.timeSamples = 0;
                    AudioSource.clip = null;
                }

                public IEnumerator FadeInPlay()
                {
                    if (playing)
                        yield break;

                    AudioSource.volume = fadeInCurve.Evaluate(0);

                    AudioSource.Play();

                    yield return parentBehivaiour.StartCoroutine(Evaluate(fadeInCurve));

                    yield break;
                }

                public void InstantPlay()
                {
                    AudioSource.Play();
                }

                public IEnumerator FadeOutPause()
                {
                    if (!playing)
                        yield break;

                    AudioSource.volume = fadeOutCurve.Evaluate(0);
                    yield return parentBehivaiour.StartCoroutine(Evaluate(fadeOutCurve));

                    AudioSource.Pause();

                    yield break;
                }

                public void InstantPause() => AudioSource.Pause();

                public IEnumerator FadeOutStopAndUnset()
                {
                    AudioSource.volume = fadeOutCurve.Evaluate(0);
                    yield return parentBehivaiour.StartCoroutine(Evaluate(fadeOutCurve));

                    AudioSource.Stop();
                    Unset();

                    yield break;
                }

                IEnumerator Evaluate(AnimationCurve curve)
                {
                    float t = 0;


#if UNITY_EDITOR // check optimised away at build
                    if (curve.length < 2)
                    {
                        throw new Exception("Fade in / out curve must have at least 2 keys");
                    }
#endif

                    var lastKey = curve[curve.length - 1];
                    while (t < lastKey.time)
                    {
                        AudioSource.volume = curve.Evaluate(t);
                        t += Time.deltaTime;
                        yield return null;
                    }
                    yield break;
                }

              
            }
        }
    }


}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rythm
{

    public class MusicManager : MonoBehaviour
    {
        private AudioSource CurrentAudioSource => multiAudioSource.CurrentTopSource;

        // private AudioSource
        [SerializeField] RythmSong defaultMusic;
        [SerializeField] MultiAudioSource multiAudioSource = new MultiAudioSource();
        [SerializeField] private bool playOnStart;

        public bool PlayingMusic => CurrentAudioSource == null ? false :  CurrentAudioSource.isPlaying;
        public int CurrentSample => CurrentAudioSource.timeSamples;
        public int CurrentClipFrequency => CurrentClip.frequency;
        public int CurrentClipSamples => CurrentClip.samples;
        private AudioClip CurrentClip => CurrentAudioSource.clip;


        private void Awake()
        {
            multiAudioSource.Init(this, GetComponent<AudioSource>());
        }

        private void Start()
        {
            if(playOnStart)
                PushNewSong(defaultMusic);
        }

        /// <summary>
        /// Play provided <paramref name="music"/> and add it to the song-stack. Pause the current song so that it can be returned to later
        /// </summary>
        public void PushNewSong(RythmSong music)
        {
            multiAudioSource.PushNewClip(music, music.beginAtSample);
            RythmEngine.Instance.SetTrackInfo(music);     
        }

        /// <summary>
        /// Play provided <paramref name="music"/> and stop the current song
        /// </summary>
        public void MutateCurrentSongToNewSong(RythmSong music)
        {
            multiAudioSource.MutateCurrentClip(music, music.beginAtSample);
            RythmEngine.Instance.SetTrackInfo(music);
        }

        /// <summary>
        /// Return to the song playing before the last call of <see cref="PushNewSong(RythmSong)"/>
        /// </summary>
        public void ReturnToPreviousSong()
        {
            var prev = multiAudioSource.ReturnToPreviousClip();
            RythmEngine.Instance.SetTrackInfo(prev);
        }

        public void PauseSong()
        {
            multiAudioSource.PauseClip();
        }

        public void ResumeSong()
        {
            multiAudioSource.ResumeClip();
        }
     


        [System.Serializable]
        private class MultiAudioSource
        {
            [SerializeField] private AnimationCurve fadeInMusicCurve;
            [SerializeField] private AnimationCurve fadeOutMusicCurve;
            public AudioSource CurrentTopSource => activeSources.Count == 0 ? null : activeSources.Peek().AudioSource;
            public RythmSong CurrentSong => activeSources.Peek().RythmSong;
            private SourceAndSong CurrentTop => activeSources.Count == 0 ? null : activeSources.Peek();

            private Stack<SourceAndSong> activeSources = new Stack<SourceAndSong>();
            private Queue<SourceAndSong> inactiveSources = new Queue<SourceAndSong>();
            private MonoBehaviour parentBehaviour;

            internal void Init(MonoBehaviour parentBehaviour, AudioSource audioSource)
            {
                if(audioSource == null)
                {
                    throw new Exception("Must have a audiosources");
                }
                this.parentBehaviour = parentBehaviour;
                inactiveSources.Enqueue(new SourceAndSong(audioSource, fadeInMusicCurve, fadeOutMusicCurve, parentBehaviour));
            }

            public RythmSong PushNewClip(RythmSong newSong, int fromSample = 0)
            {
                var newSongAndSource = GetUnusedAudioSource();
                var oldSource = CurrentTop;

                SetNewSong(newSongAndSource);

                parentBehaviour.StartCoroutine(PlayNewSong(newSongAndSource, newSong, fromSample));

                if(oldSource != null) // this will be null for first song pushed
                    parentBehaviour.StartCoroutine(PauseSong(oldSource));


                return CurrentSong;
            }


            public RythmSong MutateCurrentClip(RythmSong newSong, int fromSample = 0)
            { 
                SourceAndSong newSongAndSource = GetUnusedAudioSource();
                var oldSource = SetPreviousSong();

                SetNewSong(newSongAndSource);

                parentBehaviour.StartCoroutine(PlayNewSong(newSongAndSource, newSong, fromSample));
                parentBehaviour.StartCoroutine(StopAndKillSource(oldSource));

                return CurrentSong;
            }

          

            public RythmSong ReturnToPreviousClip()
            {
                if (!(activeSources.Count > 1))
                {
                    Debug.LogError("No past song to return to!");
                    return CurrentSong;
                }

                SourceAndSong oldSource = SetPreviousSong();
                var newTopSongAndSource = CurrentTop;

                parentBehaviour.StartCoroutine(ResumeSong(newTopSongAndSource));
                parentBehaviour.StartCoroutine(StopAndKillSource(oldSource));

                return CurrentSong;
            }

            public void PauseClip() => parentBehaviour.StartCoroutine(PauseSong(CurrentTop));

            public void ResumeClip() => parentBehaviour.StartCoroutine(ResumeSong(CurrentTop));



            ///<summary>Set current top to <paramref name="songAndSource"/></summary>
            private void SetNewSong(SourceAndSong songAndSource) => activeSources.Push(songAndSource);
            ///<summary>Set current top to element below it in the stack and return poped value</summary>
            private SourceAndSong SetPreviousSong() => activeSources.Pop();

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

            private IEnumerator PlayNewSong(SourceAndSong songAndSource, RythmSong newSong, int fromSample)
            {
                songAndSource.Set(newSong, fromSample);
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

        

            private class SourceAndSong
            {
                private readonly AnimationCurve fadeInCurve;
                private readonly AnimationCurve fadeOutCurve;
                private readonly MonoBehaviour parentBehivaiour;

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
                    AudioSource.volume = fadeInCurve.Evaluate(0);

                    AudioSource.Play();

                    yield return parentBehivaiour.StartCoroutine(Evaluate(fadeInCurve));

                    yield break;
                }

              

                public IEnumerator FadeOutPause()
                {
                    AudioSource.volume = fadeOutCurve.Evaluate(0);
                    yield return parentBehivaiour.StartCoroutine(Evaluate(fadeOutCurve));

                    AudioSource.Pause();

                    yield break;
                }

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
                    if(curve.length < 2)
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
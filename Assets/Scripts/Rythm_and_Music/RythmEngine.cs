using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SingletonManagement;
using UnityEngine.Audio;

using Helper;

namespace Rythm
{
    [RequireComponent(typeof(AudioSource))]
    public class RythmEngine : Singleton<Rythm.RythmEngine>
    {
        new public static RythmEngine Instance => Singleton<Rythm.RythmEngine>.Instance;

        public bool PlayingMusic => MusicManager.PlayingMusic;

        [SerializeField] float noMusicBPM = 120;
        float NoMusicBPS => noMusicBPM/60.0f;

        public float CurrentBeat
        {
            get
            {
                if (!PlayingMusic)
                {
                  //  Debug.LogError("Not playing music");
                    return Time.time * NoMusicBPS;
                }
                return sampleOffset + (TimeInSong * BPS);
            }
        }

        public float TimeInSong => (float)CurrentSample / (float)MusicManager.CurrentClipFrequency;
        public float DurationOfSong => (float)MusicManager.CurrentClipSamples / (float)MusicManager.CurrentClipFrequency;
        public float PercentThroughSong => TimeInSong / DurationOfSong;
        public float BPS => BPM / 60f;
        public float DurationOfBeat => BeatToSeconds(1);
        /// <summary>
        /// <see cref="Time.deltaTime"/> in beats
        /// </summary>
        public float DeltaBeats => Instance.SecondsToBeat(Time.deltaTime);

        public MusicManager MusicManager { get; private set; }

       // public AudioClip CurrentMusicClip { get => MusicManager.AudioSource.clip; }

        float BPM;
        float sampleOffset;

        float BeatsInSong => (float)(DurationOfSong * BPS);

        readonly SortedDictionary<float, Action> queuedActions = new SortedDictionary<float, Action>();


        int CurrentSample => MusicManager.CurrentSample;


      //  private AudioSource AudioSource { get; set; }

        public override void Initialise()
        {
            base.InitSingleton();
         //   AudioSource = GetComponent<AudioSource>();
            MusicManager = GetComponent<MusicManager>();
        }

        public void PlayRhytmSong(RythmSong music)
        {
            ClearAnyQueuedActions();
            MusicManager.PushNewSong(music);
           // SetTrack(music);
           // BeginPlaying(music);
        }

        public void StopRythmSong()
        {
            MusicManager.ReturnToPreviousSong();
          //  AudioSource.Stop();
          //  AudioSource.clip = null;
        }

        private void ClearAnyQueuedActions()
        {
            if(queuedActions.Count > 0)
            {
                Debug.LogError($"There are actions queued while changing the song!");
                queuedActions.Clear();
            }

        }

        //private void SetTrack(RythmSong music)
        //{
        //    AudioSource.clip = music.audioClip;
        //    SetTrackInfo(music);
        //}

        internal void SetTrackInfo(RythmSong music)
        {
            BPM = music.BPM;
            sampleOffset = music.offset;
        }

        //private void BeginPlaying(RythmSong music)
        //{
           
        //}



        //internal void DebugDetails(RythmSong music)
        //{
        //    Debug.Log($"Track: {music.name}");
        //    Debug.Log($"Samples: { music.audioClip.samples} at { music.audioClip.frequency}Hz");
        //    Debug.Log($"BPM: {BPM} ({BPS}pbs)");
        //    Debug.Log($"Duration: {TimeSpan.FromSeconds(DurationOfSong)}, {BeatsInSong} beats long");
        //    Debug.Log($"Offset: {sampleOffset} samples");

        //}

      

  

        private void Update()
        {          
            InvokeQueue();
        }

        private void InvokeQueue()
        {
            //if (!PlayingMusic)
            //{
            //    return;
            //}

            List<float> ToRemoveCache = new List<float>();
            float frameCurrentBeat = CurrentBeat;

            var cacheofQueuedActions = new SortedDictionary<float, Action>(queuedActions);

            foreach (var action in cacheofQueuedActions)
            {
                if (action.Key > frameCurrentBeat)
                {
                    continue; // break may be more efficient
                }
                action.Value?.Invoke();
                ToRemoveCache.Add(action.Key);
            }

            foreach (var key in ToRemoveCache)
            {
                queuedActions.Remove(key);
            }
        }


        public void QueueActionNextBeat(Action action) => QueueActionAfterBeats(action,0);

        /// <summary>
        /// Will add an event to be triggered on the beat, or after a certain number of beats happen. <paramref name="beatsTime"/> is measured in <paramref name="beatResolution"/> at <c>0.25 == 1/4 beats</c>
        /// </summary>
        /// <param name="action">The function to be called</param>
        /// <param name="beatsTime">The beats from now when the <paramref name="action"/> will be invoked</param>
        /// <param name="beatResolution">The sub-division of beat being used</param>
        public void QueueActionAfterBeats(Action action, int beatsTime, float beatResolution = 1)
        {
            if (!PlayingMusic)
            {
                Debug.LogWarning($"Not playing music, using fallback BMP: {noMusicBPM}");
            }

            float target;
            if(beatsTime > 0)
            {
                target = GetBeatsFromNow(beatsTime, beatResolution);
            }
            else
            {
                target = GetNextBeat(beatResolution);
            }
            
            AddEvent(action, target);
        }

        /// <summary>
        /// Will add an event to be triggered at a certain beat
        /// </summary>
        /// <param name="action">The function to be called</param>
        /// <param name="targetBeat">The beat that this will be triggered on</param>
        public void QueueActionAtExplicitBeat(Action action, float targetBeat)
        {   
            AddEvent(action, targetBeat);
        }

    

        public float GetBeatsFromNow(int beatsTime, float beatResolution = 1)
        {
            if (beatResolution <= 0)
            {
                throw new ArgumentException("Beats resolution must be greater than 0");
            }

            float nextBeat = GetNextBeat(beatResolution);

            var beatsAfterNext = beatsTime * (beatResolution); // calculate the offset

            var target = nextBeat + beatsAfterNext; // add on the offset 

            return (float)target;
        }

        private float GetNextBeat(float beatResolution = 1)
        {

            var beatSplit = Maths.WholeAndFrac(CurrentBeat); // get whole and fractional part


            var targetFrac = Maths.RoundUpTo(beatSplit.frac, beatResolution); // round up to the nearest

            var nextBeat = beatSplit.whole + targetFrac; // get the next beat
            return nextBeat;
        }

        void AddEvent(Action action, float beat)
        {
            //Debug.Log($"Queued event for beat {beat}");

            if(beat < CurrentBeat)
            {
                Debug.LogWarning("Beat queued for the past");
            }

            Action newEvent;
            if (queuedActions.TryGetValue(beat, out newEvent))
            {
               // newEvent += action;
                //queuedActions[beat] += newEvent;
                queuedActions[beat] += action;
            }
            else
            {
                newEvent += action;
                queuedActions.Add(beat, newEvent);
            }
        }

        public float BeatToSeconds(float beat) => beat / BPS;

        public float SecondsToBeat(float time) => time * BPS;


        /// <summary>
        /// Will wait a coroutine until the next beat
        /// </summary>
        /// <param name="resolution">The unit of beat to wait for</param>
        /// <returns></returns>
        public IEnumerator WaitUntilNextBeat(float resolution = 1) { yield return WaitUntilBeat(0, resolution); }

        /// <summary>
        /// Will wait a coroutine until a given beat passes 
        /// </summary>
        /// <param name="beatsFromNow">The number of beats after the next beat to wait for</param>
        /// <param name="resolution">The unit of beat to wait for</param>
        /// <returns></returns>
        public IEnumerator WaitUntilBeat(int beatsFromNow, float resolution = 1)
        {
            bool beat = false;

            Instance.QueueActionAfterBeats(() => beat = true, beatsTime: beatsFromNow, beatResolution: resolution);

            yield return new WaitUntil(() => beat);
        }

    }

}

//class TimedAction
//{
//    public readonly Action action;
//   // public readonly double beatToTrigger;
//    public bool Triggered { get; private set; } = false;

//    public TimedAction(Action action, int beatToTrigger)
//    {
//        this.action = action;
//       // this.beatToTrigger = beatToTrigger;

//        this.action += () => Triggered = true;
//    }

//    //public int CompareTo(object obj)
//    //{
//    //    return this.beatToTrigger.CompareTo(obj);
//    //}
//}

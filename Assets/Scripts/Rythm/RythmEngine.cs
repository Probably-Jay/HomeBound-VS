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

        public bool InRythmSection { get; set; } = false;

        private AudioSource AudioSource { get; set; }

        [SerializeField] RythmSong music;
        [SerializeField] bool playImediatley;
        public AudioClip MusicClip { get => music.audioClip; }
        float BPM;
        float sampleOffset;


        public override void Initialise()
        {
            base.InitSingleton();
            AudioSource = GetComponent<AudioSource>();
            SetTrack();

            DebugDetails();
            if (playImediatley)
            {
                Play();
            }
        }

        private void Play()
        {
            AudioSource.Play();
            AudioSource.timeSamples = music.beginAtSample;
            InRythmSection = true;
        }

        private void SetTrack()
        {
            AudioSource.clip = MusicClip;
            BPM = music.BPM;
            sampleOffset = music.offset;
        }

        private void DebugDetails()
        {
            Debug.Log($"Track: {music.name}");
            Debug.Log($"Samples: { MusicClip.samples} at { MusicClip.frequency}Hz");
            Debug.Log($"BPM: {BPM} ({BPS}pbs)");
            Debug.Log($"Duration: {TimeSpan.FromSeconds(Duration)}, {BeatsInSong} beats long");
            Debug.Log($"Offset: {sampleOffset} samples");

        }

        float BPS => BPM / 60f;
        float BeatsInSong => (float)(Duration * BPS);


        SortedList<float, Action> queuedActions = new SortedList<float, Action>();

        float Duration => MusicClip.samples / MusicClip.frequency;

        int CurrentSample => AudioSource.timeSamples;

        float TimeInSong => (float)CurrentSample / (float)MusicClip.frequency;

        public float CurrentBeat => sampleOffset + TimeInSong * BPS;

        /// <summary>
        /// <see cref="Time.deltaTime"/> in beats
        /// </summary>
        public float DeltaBeats => Instance.SecondsToBeat(Time.deltaTime);

        public float DurationOfBeat => BeatToSeconds(1);

        private void Update()
        {  
            SetTrack();

            InvokeQueue();
        }

        private void InvokeQueue()
        {
            List<float> ToRemoveCache = new List<float>();
            foreach (var action in queuedActions)
            {
                if (CurrentBeat < action.Key)
                {
                    break;
                }
                action.Value?.Invoke();
                ToRemoveCache.Add(action.Key);
            }

            foreach (var key in ToRemoveCache)
            {
                queuedActions.Remove(key);
            }
        }


        public void QueueActionNextBeat(Action action) => QueueActionAfterBeats(action);

        /// <summary>
        /// Will add an event to be triggered on the beat, or after a certain number of beats happen. <paramref name="beatsTime"/> is measured in <paramref name="beatResolution"/> at <c>0.25 == 1/4 beats</c>
        /// </summary>
        /// <param name="action">The function to be called</param>
        /// <param name="beatsTime">The beats from now when the <paramref name="action"/> will be invoked</param>
        /// <param name="beatResolution">The sub-division of beat being used</param>
        public void QueueActionAfterBeats(Action action, int beatsTime = 0, float beatResolution = 1)
        {
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

            var beatsAfterNext = beatsTime * (1.0 / beatResolution); // calculate the offset

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
            Debug.Log($"Queued event for beat {beat}");

            Action newEvent;
            if (queuedActions.TryGetValue(beat, out newEvent))
            {
                newEvent += action;
                queuedActions[beat] = newEvent;
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

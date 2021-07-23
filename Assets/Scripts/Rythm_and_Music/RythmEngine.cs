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

        public event Action OnSongChanged;

        [SerializeField] float noMusicBPM = 120;
        [SerializeField] private bool invokeAllQueedActionsOnSongChange;

        float NoMusicBPS => noMusicBPM / 60.0f;

        public float CurrentBeat
        {
            get
            {
                if (!PlayingMusic)
                {
                    //  Debug.LogError("Not playing music");
                    return Time.time * NoMusicBPS;
                }
                return SampleOffset + (TimeInSong * BPS);
            }
        }


        public int CurrentBar => GetBarOfBeat(CurrentBeat);
        public int CurrentBeatInBar => GetIndexInBar(CurrentBeat);
        /// <summary>
        /// Returns the index within a bar of a given beat
        /// </summary>
        public int GetIndexInBar(float beat) => (Mathf.FloorToInt(beat) + PickupBeats) % BEATS_IN_A_BAR;
        /// <summary>
        /// Returns the bar of a given beat
        /// </summary>
        public int GetBarOfBeat(float beat) => ((Mathf.FloorToInt(beat) + PickupBeats) / BEATS_IN_A_BAR);
        /// <summary>
        /// Returns the first beat of a given bar
        /// </summary>
        public int GetFirstBeatOfBar(int bar) => bar * BEATS_IN_A_BAR;


        public float TimeInSong => (float)currentEstimatedSample / FrequencyOfClip;
        private float FrequencyOfClip => (float)MusicManager.CurrentClipFrequency;

        public float DurationOfSong => (float)MusicManager.CurrentClipTotalSamples / (float)MusicManager.CurrentClipFrequency;
        public float PercentThroughSong => TimeInSong / DurationOfSong;
        public float BPS => BPM / 60f;
        public float DurationOfBeat => BeatToSeconds(1);
        /// <summary>
        /// <see cref="Time.deltaTime"/> in beats
        /// </summary>
        public float DeltaBeats => Instance.SecondsToBeat(Time.deltaTime);

        public MusicManager MusicManager { get; private set; }

        // public AudioClip CurrentMusicClip { get => MusicManager.AudioSource.clip; }

        public float BPM => MusicManager.CurrentSongBPM;
        private float SampleOffset => MusicManager.CurrentSongSampleOffset;

        private int PickupBeats => MusicManager.CurrentSongPickupBeats;

        public const int BEATS_IN_A_BAR = 4;


        public float BeatsInSong => (float)(DurationOfSong * BPS);


        readonly SortedDictionary<float, Action> queuedActions = new SortedDictionary<float, Action>();

        private float SamplesToBeats(int sample) => SamplesToSeconds(sample) / BPS;

        public float SamplesToSeconds(int sample) => sample / FrequencyOfClip;


        int CurrentMusicSample => MusicManager.CurrentSample;

        int currentEstimatedSample;
        int cachedMusicSample;

        RollingAverage rollingAverageSamplesOff = new RollingAverage(30);

        //  private AudioSource AudioSource { get; set; }

        private void Update()
        {
            InvokeQueue();
            UpdateCurrentSample();
        }

        private void UpdateCurrentSample()
        {
            if (!PlayingMusic) return;

            UpdateCurrentSampleEstimate();

            if(cachedMusicSample == CurrentMusicSample)
            {
                return;
            }


            int difference = currentEstimatedSample - CurrentMusicSample;

            if(Mathf.Abs(difference) > SecondsToSamples(3))
            {
                Debug.LogWarning($"Huge sample difference spike detected: {SamplesToSeconds(difference)}");
            }

            rollingAverageSamplesOff.Record(difference);

            if(!rollingAverageSamplesOff.ReachedAccuracy)
            {
                return;
            }

            var avg = Mathf.Abs(rollingAverageSamplesOff.SmallestAbsoluteValue);

            // Debug.Log($"avg: {avg}");

            int sampleThreshold = SecondsToSamples(0.005f);

            if (avg > sampleThreshold) // correct any differences in sample
            {
                if(SamplesToSeconds(Mathf.Abs(difference)) > 2)
                {
                    Debug.LogError($"Huge sample jump");
                }
                if(Mathf.Abs(difference) < sampleThreshold)
                {
                    Debug.LogError($"Minuscule sample jump");
                }
                MusicManager.SyncSongTime(currentEstimatedSample, difference);
            }

            cachedMusicSample = CurrentMusicSample;

        }

        internal void SetCachedDataToNewTrack()
        {
            cachedMusicSample = CurrentMusicSample;
            currentEstimatedSample = CurrentMusicSample;
            rollingAverageSamplesOff.Reset();
        }


        internal void ReSync()
        {
            SetCachedDataToNewTrack();
        }

        public int SecondsToSamples(float s) => Mathf.RoundToInt((float)FrequencyOfClip * s);

        [System.Obsolete()]
        private void LegacyUpdateCurrentSample()
        {
            if (cachedMusicSample == CurrentMusicSample)
            {
                UpdateCurrentSampleEstimate();
                return;
            }

            cachedMusicSample = CurrentMusicSample;

            int samplesDelta = CurrentMusicSample - currentEstimatedSample;
            if (currentEstimatedSample < CurrentMusicSample) { }
            //  Debug.LogWarning($"Time jumping forwards by {samplesDelta} ({SamplesToBeats(samplesDelta)} beats) to ({currentEstimatedSample} to {CurrentMusicSample})");
            else { }
            // Debug.LogError($"***Time jumping backwards by {samplesDelta} ({SamplesToBeats(samplesDelta)} beats) to ({currentEstimatedSample} to {CurrentMusicSample})***");



            currentEstimatedSample = CurrentMusicSample;
        }



        private void UpdateCurrentSampleEstimate()
        {
            var ds = Mathf.RoundToInt(Time.unscaledDeltaTime * FrequencyOfClip);
            currentEstimatedSample += ds;
        }

        public override void Initialise()
        {
            base.InitSingleton();
            //   AudioSource = GetComponent<AudioSource>();
            MusicManager = GetComponent<MusicManager>();
            MusicManager.OnChangedMusic += MusicManager_OnChangedMusic;
        }


        private void OnDisable()
        {
            MusicManager.OnChangedMusic -= MusicManager_OnChangedMusic;
        }
        private void MusicManager_OnChangedMusic(RythmSong song)
        {
            ClearAnyQueuedActions();
            SetCachedDataToNewTrack();
            OnSongChanged?.Invoke();
        }

        public void PlaySong(RythmSong music)
        {
            MusicManager.PushNewSong(music);
        }

        public void StopSong()
        {
            MusicManager.ReturnToPreviousSong();
        }

        public void PlayRhythmSectionSong(SplitRythmSong music)
        {          
            MusicManager.StartRhythmSection(music);
            ResumeSongMelody();
        }

        public void ResumeSongMelody() => MusicManager.ResumeMelody();

        public void PauseSongMelody() => MusicManager.PauseMelody();

        public void StopRhythmSectionSong()
        {
            MusicManager.StopRythmSection();
        }

        private void ClearAnyQueuedActions()
        {
            if (queuedActions.Count > 0)
            {
                if (invokeAllQueedActionsOnSongChange)
                {
                    Debug.LogWarning($"There are actions queued while changing the song! Invoking all");
                    InvokeAllActions();
                }
                else
                {
                    Debug.LogError($"There are actions queued while changing the song! Discarding all");
                }

                queuedActions.Clear();
            }

        }

        private void InvokeAllActions()
        {
            int count = 0;
            while (queuedActions.Count > 0)
            {
                if (count > 100)
                {
                    throw new Exception("Possible cyclical action queue detected");
                    // break;
                }
                List<float> ToRemoveCache = new List<float>();
                List<Action> ToInvokeCache = new List<Action>();
                var cacheofQueuedActions = new SortedDictionary<float, Action>(queuedActions);
                foreach (var action in cacheofQueuedActions)
                {
                    ToInvokeCache.Add(action.Value);
                    ToRemoveCache.Add(action.Key);
                }

                foreach (var key in ToRemoveCache)
                {
                    queuedActions.Remove(key);
                }

                foreach (var action in ToInvokeCache) // we need to make sure it's removed *BEFORE* it's invoked
                {
                    action?.Invoke();
                }


                count++;
            }
        }



   


        private void InvokeQueue()
        {
            //if (!PlayingMusic)
            //{
            //    return;
            //}

            List<float> ToRemoveCache = new List<float>();
            List<Action> ToInvokeCache = new List<Action>();
            float frameCurrentBeat = CurrentBeat;

            var cacheofQueuedActions = new SortedDictionary<float, Action>(queuedActions);

            foreach (var action in cacheofQueuedActions)
            {
                if (action.Key > frameCurrentBeat)
                {
                    continue; // break may be more efficient
                }
                ToInvokeCache.Add(action.Value);
                ToRemoveCache.Add(action.Key);
            }

            foreach (var key in ToRemoveCache)
            {
                queuedActions.Remove(key);
            }

            foreach (var action in ToInvokeCache) // invoke *AFTER* removed
            {
                action?.Invoke();
            }
        }


        public void QueueActionNextBeat(Action action) => QueueActionAfterBeats(action, 0);
        public void QueueActionNextBer(Action action) => QueueActionAfterBars(action, 0);

     

        /// <summary>
        /// Will add an event to be triggered on the beat, or after a certain number of beats happen. <paramref name="beatsTime"/> is measured in <paramref name="beatResolution"/> at <c>0.25 == 1/4 beats</c>
        /// </summary>
        /// <param name="action">The function to be called</param>
        /// <param name="beatsTime">The beats from the next after which the <paramref name="action"/> will be invoked</param>
        /// <param name="beatResolution">The sub-division of beat being used</param>
        public void QueueActionAfterBeats(Action action, int beatsTime, float beatResolution = 1)
        {
            if (!PlayingMusic)
            {
                Debug.LogWarning($"Not playing music, using fallback BMP: {noMusicBPM}");
            }

            float target;
            if (beatsTime > 0) // more than one beat
            {
                target = GetBeatsFromNow(beatsTime, beatResolution);
            }
            else
            {
                target = GetNextBeat(beatResolution); // the next beat
            }

            AddEvent(action, target);
        }

       

        /// <summary>
        /// Will add an event to be triggered on the first beat of the next bar, or the first beat a certain number of bars after the next bar.
        /// </summary>
        /// <param name="action">The function to be called</param>
        /// <param name="barsTime">The bars from after the next that the <paramref name="action"/> will be invoked</param>
        public void QueueActionAfterBars(Action action, int barsTime)
        {
            if (!PlayingMusic)
            {
                Debug.LogWarning($"Not playing music, using fallback BMP: {noMusicBPM}");
            }

            float target;
            if (barsTime > 0) // more than one beat
            {
                target = GetBarsFromNow(barsTime);
            }
            else
            {
                target = GetNextBar(); // the next beat
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

        /// <summary>
        /// Will add an event to be triggered on the first beat of a specified bar
        /// </summary>
        /// <param name="action">The function to be called</param>
        /// <param name="targetBar">The bar that this will be triggered on</param>
        public void QueueActionAtExplicitBar(Action action, int targetBar)
        {
            var targetBeat = GetFirstBeatOfBar(targetBar);
            AddEvent(action, targetBeat);
        }


        /// <summary>
        /// Get the beat <paramref name="beatsTime"/> after the next beat (at <paramref name="beatResolution"/> resolution). Passing <paramref name="beatsTime"/> = 0 will have the same effect as calling <see cref="GetNextBeat(float)"/>
        /// </summary>
        /// <returns></returns>
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

        public float GetNextBeat(float beatResolution = 1)
        {
            var beatSplit = Maths.WholeAndFrac(CurrentBeat); // get whole and fractional part

            var targetFrac = Maths.RoundUpTo(beatSplit.frac, beatResolution); // round up to the nearest

            var nextBeat = beatSplit.whole + targetFrac; // get the next beat
            return nextBeat;
        }

        /// <summary>
        /// Get the first beat of the bar <paramref name="barsTime"/> bars after the next bar. Passing <paramref name="barsTime"/> = 0 will have the same effect as calling <see cref="GetNextBar()"/>
        /// </summary>
        public float GetBarsFromNow(int barsTime)
        {
            float firstBeatOfnextBar = GetNextBar();

            float beatsOfBarsAfterNext = barsTime * BEATS_IN_A_BAR;

            var target = firstBeatOfnextBar + beatsOfBarsAfterNext; // add on the offset 

            return (float)target;
        }

        public float GetNextBar()
        {
            var nextBeat = GetNextBeat();
            var nextBeatBarIndex = GetIndexInBar(nextBeat);
            var beatsAfterNextTilNextBar = (BEATS_IN_A_BAR - nextBeatBarIndex) % BEATS_IN_A_BAR;

            return nextBeat + beatsAfterNextTilNextBar;
        }
        void AddEvent(Action action, float beat)
        {
            if (beat < CurrentBeat)
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

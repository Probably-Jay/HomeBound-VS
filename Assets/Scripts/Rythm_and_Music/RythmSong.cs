using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEditor;


namespace Rythm
{
    [CreateAssetMenu(fileName = "Song", menuName = "ScriptableObjects/Rhythm/Song", order = 1)]
    public class RythmSong : ScriptableObject
    {
        public AudioClip mainRhythmAudioClip;
        [Range(1,360)] public float BPM;
        [Range(-1,1)]public float offset;
        public int beginAtSample => Mathf.RoundToInt(beginAtTime * mainRhythmAudioClip.frequency);

        public float beginAtTime;
    }   
    
    
    [CreateAssetMenu(fileName = "Song", menuName = "ScriptableObjects/Rhythm/Split Song", order = 1)]
    public class SplitRythmSong
    {
        public RythmSong melodySong;
        public RythmSong backingRhythmSong;

    }




}
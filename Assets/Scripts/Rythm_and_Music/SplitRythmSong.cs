using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEditor;


namespace Rythm
{
  
    [CreateAssetMenu(fileName = "SplitSong", menuName = "ScriptableObjects/Rhythm/SplitSong", order = 1)]
    public class SplitRythmSong : ScriptableObject
    {
        public RythmSong melodyRhythmSong;
        public RythmSong backingSong;
    }

}
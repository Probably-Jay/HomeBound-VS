using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEditor;


namespace Rythm
{
    [CreateAssetMenu(fileName = "NoteSection", menuName = "ScriptableObjects/NoteSection", order = 1)]
    public class NoteSection : ScriptableObject
    {
        public RythmSong song;
        public TextAsset notes;
    }
}
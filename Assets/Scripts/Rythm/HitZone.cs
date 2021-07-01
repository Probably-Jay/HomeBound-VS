using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NoteSystem;

namespace NoteSystem
{

        /*
        public enum HitQuality
        {
            Miss
            , Early
            , Late
            , Good
            , Great
            , Perfect
        }
        */
        public class HitZone : MonoBehaviour
        {
            [SerializeField] List<WordNote> notesInChannel = new List<WordNote> { };
           // [SerializeField] KeyCode button = KeyCode.Space;
            // Start is called before the first frame update
           
            
            void ProcessHitOnNote(WordNote note, HitQuality quality)
            {
                note.Remove();
            }



        }
    }




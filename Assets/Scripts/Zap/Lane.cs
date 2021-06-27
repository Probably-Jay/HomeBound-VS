using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NoteSystem;

namespace NoteSystem
{




    public class Lane : MonoBehaviour
    {
        [SerializeField] private Transform editorSP;
        [SerializeField] private HitZone editorHZ;
        [SerializeField] public Transform spawnPoint { get; private set; }
        [SerializeField] public HitZone hitZone { get; private set; }
        [SerializeField] List<WordNote> notesInChannel = new List<WordNote> { };
        [SerializeField] KeyCode button = KeyCode.Space;
        [SerializeField] NoteDialogueInterface NDI;

        // Start is called before the first frame update
        void Awake()
        {
            spawnPoint = editorSP;
            hitZone = editorHZ;
        }
        public void Join(WordNote note)
        {
            notesInChannel.Add(note);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(button))
            {
            //    Debug.Log("bam!");
                Rythm.RythmEngine.Instance.QueueActionNextBeat(() =>
                {
                    //Debug.Log("beat");
                }
                    );
                float tempCurrentBeat = Rythm.RythmEngine.Instance.CurrentBeat;
                RemovePastBeats(tempCurrentBeat);
                ProcessHitOnNextNote(tempCurrentBeat);
                if (notesInChannel.Count > 0)
                {

                }
            }
        }
        private void RemovePastBeats(float tempCurrentBeat)
        {
            if (notesInChannel.Count > 0)
            {
                HitQuality hitKind = HitDetection.CheckHit((float)notesInChannel[0].GetClimaxBeat(), tempCurrentBeat);
                if ((notesInChannel[0].GetClimaxBeat() < Rythm.RythmEngine.Instance.CurrentBeat) && (hitKind == HitQuality.Miss))
                {
                    notesInChannel.RemoveAt(0);
                    RemovePastBeats(tempCurrentBeat);
                }
            }
        }
        private void ProcessHitOnNextNote(float tempCurrentBeat)
        {
            if (notesInChannel.Count > 0)
            {
                HitQuality nextNoteHitQuality = HitDetection.CheckHit((float)notesInChannel[0].GetClimaxBeat(), tempCurrentBeat);
                if (nextNoteHitQuality == HitQuality.Miss)
                {
                    //process miss here
                    //Debug.Log("Miss!");
                  //  Debug.Log(tempCurrentBeat + ", " + notesInChannel[0].GetClimaxBeat());
                    return;
                }
                else
                {
                 //   Debug.Log(nextNoteHitQuality);
                    ProcessHitOnNote(notesInChannel[0], nextNoteHitQuality);
                    notesInChannel.RemoveAt(0);
                    Debug.Log("processed hit on:" + notesInChannel[0].word);
                }

            }
        }
        void ProcessHitOnNote(WordNote note, HitQuality quality)
        {
            //  Debug.Log(note.word);
            
            if (note.GetClimaxBeat() < Rythm.RythmEngine.Instance.CurrentBeat)
            {
                NDI.AddWord(note.word, note.GetClimaxBeat());
            }
            else
            {
                NDI.AddWord(note.word);
            }
            note.Remove();

        }

    }
}





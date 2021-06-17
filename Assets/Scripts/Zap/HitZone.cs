using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NoteSystem;

namespace HitDetection {
    public enum HitQuality
    {
        Miss
        ,Early
        ,Late
        ,Good
        ,Great
        ,Perfect
    }
    public class HitZone : MonoBehaviour
    {
        [SerializeField] List<WordNote> notesInChannel = new List<WordNote> { };
        [SerializeField] KeyCode button = KeyCode.Space;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(button))
            {
                Debug.Log("bam!");
                Rythm.RythmEngine.Instance.QueueActionNextBeat(() => {
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
                HitQuality hitKind = CheckHit((float)notesInChannel[0].GetClimaxBeat(), tempCurrentBeat);
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
                HitQuality nextNoteHitQuality = CheckHit((float) notesInChannel[0].GetClimaxBeat(), tempCurrentBeat);
                if (nextNoteHitQuality == HitQuality.Miss)
                {
                    //process miss here
                    Debug.Log("Miss!");
                    Debug.Log(tempCurrentBeat +", "+ notesInChannel[0].GetClimaxBeat());
                    return;
                }
                else
                {
                    Debug.Log(nextNoteHitQuality);
                    ProcessHitOnNote(notesInChannel[0], nextNoteHitQuality);
                }
                
            }
        }
        void ProcessHitOnNote(WordNote note, HitQuality quality)
        {
            note.Remove();
        }


        public static HitQuality CheckHit(float targetBeat, float currentBeat)
        {
            float beatsOff = (Mathf.Abs(targetBeat - currentBeat));
            
            if (beatsOff > 0.4){return HitQuality.Miss;}

            //HitQuality output;
            if (beatsOff < 0.1)
            {
                return HitQuality.Perfect;
            }           
            else if (beatsOff < 0.2)
            {
                return HitQuality.Great;
            }
            else if (beatsOff < 0.3)
            {
                return HitQuality.Good;
            }
            else if (targetBeat > currentBeat)
            {
                return HitQuality.Early;
            }
            else
            {
                return HitQuality.Late;
            }
                               
        }
        public void Join(WordNote note)
        {
            notesInChannel.Add(note);   
        }
    }
    }
    


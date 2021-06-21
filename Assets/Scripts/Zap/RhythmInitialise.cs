using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NoteSystem;
namespace RhythmSectionLoading {
    class Note
    {
        public float climaxBeat { get; private set; }
        public int lane { get; private set; }
        public string word { get; private set; }
        public void Initialise(float climaxBeat, int lane, string word)
        {
            this.climaxBeat = climaxBeat;
            this.lane = lane;
            this.word = word;
        }
    }

    public class RhythmInitialise : MonoBehaviour
    {
        
        public enum NoteDetails
        {
                ClimaxBeat
                , Lane
                , word
        }
        [SerializeField] TextAsset noteSheet;
        string[] noteSheetLines;
        [SerializeField] Rythm.RythmSong song;
        NoteSpawner noteSpawner;
        public List<Lane> lanes = new List<Lane> { };
        List<Note> notes = new List<Note> { };
        [SerializeField] float leadTime = 5f;
        // Start is called before the first frame update
        void Start()
        {
            noteSpawner = this.GetComponent<NoteSpawner>();
            /*
            Rythm.RythmEngine.Instance.QueueActionNextBeat(() => {
                noteSpawner.SpawnNote("bumsex", 42, lanes[1]);
            }
                        );
            Rythm.RythmEngine.Instance.QueueActionNextBeat(() => {
                noteSpawner.SpawnNote("bumsex", 42, lanes[1]);
            }
                        );
                        */
            LoadSection();
            //float offset = -4f;
            /*
            Rythm.RythmEngine.Instance.QueueActionAtExplicitBeat(() => { noteSpawner.SpawnNote("It's", 42 + offset, lanes[0]); }, 42 + offset - leadTime);
            Rythm.RythmEngine.Instance.QueueActionAtExplicitBeat(() => { noteSpawner.SpawnNote("a", 42.25f + offset, lanes[1]); }, 42.25f + offset - leadTime);
            Rythm.RythmEngine.Instance.QueueActionAtExplicitBeat(() => { noteSpawner.SpawnNote("beautiful", 42.5f + offset, lanes[2]); }, 42.5f + offset - leadTime);
            Rythm.RythmEngine.Instance.QueueActionAtExplicitBeat(() => { noteSpawner.SpawnNote("day", 43f + offset, lanes[0]); }, 43f + offset - leadTime);
            Rythm.RythmEngine.Instance.QueueActionAtExplicitBeat(() => { noteSpawner.SpawnNote("outside", 43.75f + offset, lanes[0]); }, 43.75f + offset - leadTime);
            Rythm.RythmEngine.Instance.QueueActionAtExplicitBeat(() => { noteSpawner.SpawnNote("birds", 44.25f + offset, lanes[1]); }, 44.25f + offset - leadTime);
            Rythm.RythmEngine.Instance.QueueActionAtExplicitBeat(() => { noteSpawner.SpawnNote("are", 44.5f + offset, lanes[2]); }, 44.5f + offset - leadTime);
            Rythm.RythmEngine.Instance.QueueActionAtExplicitBeat(() => { noteSpawner.SpawnNote("singing", 45.0f + offset, lanes[0]); }, 45.0f + offset - leadTime);
            Rythm.RythmEngine.Instance.QueueActionAtExplicitBeat(() => { noteSpawner.SpawnNote("flowers", 45.250f + offset, lanes[1]); }, 45.25f + offset - leadTime);
            Rythm.RythmEngine.Instance.QueueActionAtExplicitBeat(() => { noteSpawner.SpawnNote("are", 45.5f + offset, lanes[2]); }, 45.5f + offset - leadTime);
            Rythm.RythmEngine.Instance.QueueActionAtExplicitBeat(() => { noteSpawner.SpawnNote("blooming", 45.750f + offset, lanes[0]); }, 45.75f + offset - leadTime);
            */
            //Rythm.RythmEngine.Instance.QueueActionAtExplicitBeat(() => { noteSpawner.SpawnNote("dad", 44, lanes[1]); }, 34);
            

        }

        // Update is called once per frame
        void Update()
        {

        }
        void LoadSection()
        {
            ReadSection(noteSheet);
            InitialiseSection();

        }
        void ReadSection(TextAsset text)
        {
            notes.Clear();
            noteSheetLines = text.text.Split('\n');
            foreach(string line in noteSheetLines)
            {
                Debug.Log(line);
                string hitbeatString = line.Split(',')[0];
                
                string laneString = line.Split(',')[1];
                string temp1 = line.Remove(0, line.IndexOf(',')+1);
                Debug.Log(temp1);
                string word = temp1.Remove(0, temp1.IndexOf(',')+1);
                notes.Add(new Note());
                float hitBeat = float.Parse(hitbeatString);
                int lane = int.Parse(laneString);
                notes[notes.Count - 1].Initialise(hitBeat, lane, word);
                Debug.Log(word);
                Debug.Log("Note Read: "+hitbeatString + "," + laneString + "," + word);
            }

        }
        void InitialiseSection()
        {
            foreach(Note note in notes)
            {
                QueueNote(note);
            }
        }
        void QueueNote(Note note)
        {
            Rythm.RythmEngine.Instance.QueueActionAtExplicitBeat(() => { noteSpawner.SpawnNote(note.word, note.climaxBeat, lanes[note.lane]); }, note.climaxBeat - leadTime);
        }

    }
}

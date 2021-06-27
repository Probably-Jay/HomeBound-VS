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
    
            LoadSection();
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
                //Debug.Log(line); //40,1,hello
                string hitbeatString = line.Split(',')[0]; //40
                string laneString = line.Split(',')[1];//1
                string temp1 = line.Remove(0, line.IndexOf(',')+1);//1,hello
                //Debug.Log(temp1);
                string word = temp1.Remove(0, temp1.IndexOf(',')+1);//hello
                word = word.Replace("\r", "");
                //Debug.Log(word);
                notes.Add(new Note());
                float hitBeat = float.Parse(hitbeatString);
                int lane = int.Parse(laneString);
                notes[notes.Count - 1].Initialise(hitBeat, lane, word);
                //Debug.Log(word);
                //Debug.Log("Note Read: "+hitbeatString + "," + laneString + "," + word);
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

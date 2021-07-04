using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NoteSystem;
namespace RhythmSectionLoading {
    enum CommandType
    {
        EndSection
    }
    public class RhythmInitialise : MonoBehaviour
    {
        
        public enum NoteDetails
        {
                ClimaxBeat
                , Lane
                , word
        }
        TextAsset noteSheet;
        string[] noteSheetLines;
        //[SerializeField] Rythm.RythmSong song;
        NoteSpawner noteSpawner;
        [SerializeField] RhythmSectionManager rSM;
        public List<Lane> lanes = new List<Lane> { };
        List<Note> notes = new List<Note> { };
        List<PassToDialogue> toDialogues = new List<PassToDialogue> { };
        List<OtherCommand> otherCommands = new List<OtherCommand> { };
        [SerializeField] float leadTime = 5f;
        [SerializeField] bool debugStartOnStart;

        [SerializeField] GameObject childObject;


        // Start is called before the first frame update
        void Start()
        {
            noteSpawner = this.GetComponent<NoteSpawner>();
            if (!debugStartOnStart)
            {
                SwitchOffLanes();
            }

            //LoadSection(noteSheet);
        }



        void SwitchOnLanes()
        {
            childObject.SetActive(true);
        }

        void SwitchOffLanes()
        {
            childObject.SetActive(false);
        }

        public void LoadAndBeginSectionNotes(TextAsset section)
        {
            SwitchOnLanes();
            noteSheet = section;
            ReadSection(noteSheet);
            InitialiseSection();

        }

        public void EndSection()
        {
            SwitchOffLanes();
            noteSheet = null;
            notes.Clear();
            toDialogues.Clear();
            otherCommands.Clear();
        }

        void ReadSection(TextAsset text)
        {
            notes.Clear();
            toDialogues.Clear();
            otherCommands.Clear();
            noteSheetLines = text.text.Split('\n');
            foreach (string line in noteSheetLines)
            {
                if (line[0] == '>') // pass to dialogue
                {
                    string passBeatString = line.Split(',')[1];
                    string returnBeatString = line.Split(',')[2];
                    float passBeat = float.Parse(passBeatString);
                    float returnBeat = float.Parse(returnBeatString);
                    toDialogues.Add(new PassToDialogue());
                    toDialogues[toDialogues.Count - 1].Initialise(passBeat, returnBeat);
                }
                else if (line[0] == ']') // end section
                {
                    string onbeatString = line.Split(',')[1];
                    float onBeat = float.Parse(onbeatString);
                    otherCommands.Add(new OtherCommand());
                    otherCommands[otherCommands.Count - 1].Initialise(onBeat, CommandType.EndSection);
                }
                else
                {
                    //Debug.Log(line); //40,1,hello
                    string hitbeatString = line.Split(',')[0]; //40
                    string laneString = line.Split(',')[1];//1
                    string temp1 = line.Remove(0, line.IndexOf(',') + 1);//1,hello
                                                                         //Debug.Log(temp1);
                    string word = temp1.Remove(0, temp1.IndexOf(',') + 1);//hello
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

        }
        void InitialiseSection()
        {
            foreach(Note note in notes)
            {
                QueueNote(note);
            }
            foreach(PassToDialogue toDialogue in toDialogues)
            {
                QueueDialoguePass(toDialogue);
            }
            foreach (OtherCommand command in otherCommands)
            {
                QueueCommand(command);
            }
        }
        void QueueNote(Note note)
        {
            Rythm.RythmEngine.Instance.QueueActionAtExplicitBeat(() => { noteSpawner.SpawnNote(note.word, note.climaxBeat, lanes[note.lane]); }, note.climaxBeat - leadTime);
        }
        void QueueDialoguePass(PassToDialogue toDialogue)
        {
            Rythm.RythmEngine.Instance.QueueActionAtExplicitBeat(() => { rSM.PassToDialogue(toDialogue.returnBeat); }, toDialogue.passBeat);
        }
        void QueueCommand(OtherCommand command)
        {
            if (command.type == CommandType.EndSection)
            {
                Rythm.RythmEngine.Instance.QueueActionAtExplicitBeat(() => { rSM.EndSection(); }, command.onBeat);
            }
        }

    }
  
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
    class PassToDialogue
    {
        public float passBeat { get; private set; }
        public float returnBeat { get; private set; }
        public void Initialise(float passBeat, float returnBeat)
        {
            this.passBeat = passBeat;
            this.returnBeat = returnBeat;
        }
    }
    class OtherCommand
    {
        public CommandType type { get; private set; }
        public float onBeat { get; private set; }
        public void Initialise(float onBeat, CommandType type)
        {
            this.onBeat = onBeat;
            this.type = type;
        }
    }


}

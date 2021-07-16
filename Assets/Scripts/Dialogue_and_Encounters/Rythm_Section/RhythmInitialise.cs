using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NoteSystem;
using System.Linq;
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
        List<string> sectionLines = new List<string> { };
        [SerializeField] float leadTime = 5f;
        [SerializeField] bool debugStartOnStart;
        List<List<Note>> noteLines = new List<List<Note>> { };
        [SerializeField] GameObject childObject;


        // Start is called before the first frame update
        void Start()
        {
            noteSpawner = this.GetComponent<NoteSpawner>();
            noteSpawner.Init(rSM);
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
            sectionLines = SplitSectionIntoStringLines(notes, toDialogues, otherCommands);
            noteLines = SplitSectionIntoNoteLines(notes, toDialogues, otherCommands);
            foreach(string sectionLine in sectionLines)
            {
                Debug.Log(sectionLine);
            }
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
                else if (line[0]=='a')//anchor points/absolute points
                {
                    //Debug.Log(line); //40,1,hello
                    string hitbeatString = line.Split(',')[1]; //40
                    string laneString = line.Split(',')[2];//1
                    string temp1 = line.Remove(0, line.IndexOf(',') + 1);//1,hello
                    string temp2 = temp1.Remove(0, line.IndexOf(',') + 1);//1,hello                                //Debug.Log(temp1);
                    string word = temp2.Remove(0, temp1.IndexOf(',') + 1);//hello
                    word = word.Replace("\r", "");
                    //Debug.Log(word);
                    notes.Add(new Note());
                    float hitBeat;
                    try
                    {
                        hitBeat =float.Parse(hitbeatString);
                    }
                    catch
                    {
                        string possibleError = "";
                        if (hitbeatString == ">")
                        {
                            possibleError = " did you try to make a pass command absolute?";
                        }
                        throw new System.Exception("Attempted to parse \"" + hitbeatString + "\" as a climax beat." + possibleError);
                    }
                    int lane = int.Parse(laneString);
                    notes[notes.Count - 1].Initialise(hitBeat, lane, word);
                    //Debug.Log(word);
                    //Debug.Log("Note Read: "+hitbeatString + "," + laneString + "," + word);
                }
                else
                {
                    string relativeHitBeatString = line.Split(',')[0]; //40
                    string laneString = line.Split(',')[1];//1
                    string temp1 = line.Remove(0, line.IndexOf(',') + 1);//1,hello
                                                                         //Debug.Log(temp1);
                    string word = temp1.Remove(0, temp1.IndexOf(',') + 1);//hello
                    word = word.Replace("\r", "");
                    //Debug.Log(word);
                    notes.Add(new Note());
                    if (notes.Count < 2)
                    {
                        throw new System.Exception("Relative note attempted to parse as first note. Please use an absolute note as the first note");
                    }
                    float hitBeat = float.Parse(relativeHitBeatString)+notes[notes.Count-2].climaxBeat;
                    int lane = int.Parse(laneString);
                    notes[notes.Count - 1].Initialise(hitBeat, lane, word);
                }
            }

        }
        List<string> SplitSectionIntoStringLines(List<Note> notes, List<PassToDialogue> passes,List<OtherCommand> commands)
        {
            List<string> secLines = new List<string> { };
            List<float> lineEnds = new List<float> { };
            foreach (PassToDialogue pass in passes)
            {
                lineEnds.Add(pass.returnBeat);          
            }
            foreach(OtherCommand command in commands)
            {
                if (command.type == CommandType.EndSection)
                {
                    lineEnds.Add(command.onBeat);
                }
            }
            lineEnds.Sort();
            foreach(int end in lineEnds)
            {
                secLines.Add("");
            }
            notes = notes.OrderBy(note => note.climaxBeat).ToList();
            foreach(Note note in notes)
            {
                for(int i=0; i < lineEnds.Count; i++)
                {
                    if (note.climaxBeat < lineEnds[i])
                    {
                        secLines[i] += note.word + " ";
                        i = lineEnds.Count;
                    }
                }
            }
            return secLines;
        }
        List<List<Note>> SplitSectionIntoNoteLines(List<Note> notes, List<PassToDialogue> passes, List<OtherCommand> commands)
        {
            List<List<Note>> secLines = new List<List<Note>> { };
            List<float> lineEnds = new List<float> { };
            foreach (PassToDialogue pass in passes)
            {
                lineEnds.Add(pass.returnBeat);
            }
            foreach (OtherCommand command in commands)
            {
                if (command.type == CommandType.EndSection)
                {
                    lineEnds.Add(command.onBeat);
                }
            }
            lineEnds.Sort();
            foreach (int end in lineEnds)
            {
                secLines.Add(new List<Note> { });
            }
            notes = notes.OrderBy(note => note.climaxBeat).ToList();
            foreach (Note note in notes)
            {
                for (int i = 0; i < lineEnds.Count; i++)
                {
                    if (note.climaxBeat < lineEnds[i])
                    {
                        secLines[i].Add(note);
                        i = lineEnds.Count;
                    }
                }
            }
            return secLines;
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
            QueueLinePreviews();
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
        void QueueLinePreviews()
        {
            for (int i = 0; i < sectionLines.Count; i++)
            {

                float targetBeat = noteLines[i][0].climaxBeat - leadTime;
                if (targetBeat < 0)
                {
                    targetBeat = 0;
                }
                string line = sectionLines[i];
                Rythm.RythmEngine.Instance.QueueActionAtExplicitBeat(() => { rSM.ShowPreviewLine(line); }, targetBeat);
            }
        }
        //this, along with it's accompanying adding method, is deprecated, because as MATT informs me, this is replacable by one system.linq line. : ) I'm fine.
        List<Note> SortNoteListByClimaxBeat(List<Note> notes)
        {
            List<Note> sortedNotes = new List<Note> { };
            if (notes[0] == null)
            {
                return notes;
            }
            sortedNotes.Add(notes[0]);
            for (int i = 1; i < notes.Count; i++)
            {
                AddSortedNote(notes[i], sortedNotes);
            }
            return sortedNotes;
        }

        //i said i'M fINE
        private static void AddSortedNote(Note note, List<Note> sortedNotes)
        {
            bool added = false;
            for (int j = 0; j < sortedNotes.Count; j++)
            {
                if (note.climaxBeat < sortedNotes[j].climaxBeat)
                {
                    if (added == false)
                    {
                        sortedNotes.Insert(j, note);
                    }
                    added = true;
                    j = sortedNotes.Count;
                }
            }
            if (added == false)
            {
                sortedNotes.Add(note);
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

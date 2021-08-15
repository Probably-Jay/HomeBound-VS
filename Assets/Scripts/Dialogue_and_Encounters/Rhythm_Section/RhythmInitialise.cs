using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NoteSystem;
using System.Linq;
using System;
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

            for (int i = 0; i < noteSheetLines.Length; i++)
            {
                string line = noteSheetLines[i];

                if (line == null || line == "")
                {
                    continue;
                }

                try
                {
                    ParseLine(line);
                }
                catch
                {
                    Debug.LogError($"Error reading line {i+1} in notesection {text.name}");
                    throw;
                }
            }

        }

        private void ParseLine(string line)
        {
            if (line.Length == 0)
            {
                throw new Exception("Line is empty");
            }

            var segments = line.Split(',');

            if (segments.Length < 2)
            {
                throw new Exception("Not enough segments, possibly missing comma");
            }


            if (line[0] == '>') // pass to dialogue
            {
                ParsePassToDialogue(line, segments);
            }
            else if (line[0] == ']') // end section
            {
                ParseEndSection(line, segments);
            }
            else if (line[0] == 'a')//anchor points/absolute points
            {
                ParseAbsolute(line, segments);
            }
            else
            {
                ParseRelative(line, segments);
            }
        }

        private void ParseRelative(string line, string[] segments)
        {
            string relativeHitBeatString = segments[0]; //40
            string laneString = segments[1];//1

            string temp1 = line.Remove(0, line.IndexOf(',') + 1);//1,hello
                                                                 //Debug.Log(temp1);
            string word = temp1.Remove(0, temp1.IndexOf(',') + 1);//hello

            word = word.Replace("\r", "");

            if (word.Length == 0 || word == "\n")
            {
                throw new Exception("No text in section");
            }

            notes.Add(new Note());
            if (notes.Count < 2)
            {
                throw new System.Exception("Relative note attempted to parse as first note. Please use an absolute note as the first note");
            }

            float beat;
            int lane;
            try
            {
                beat = float.Parse(relativeHitBeatString);
                lane = int.Parse(laneString);
            }
            catch 
            {
                Debug.LogError("Could not parse floats / floats in section");
                throw;
            }
            ValidateLane(lane);
            float hitBeat = beat + notes[notes.Count - 2].climaxBeat;
            notes[notes.Count - 1].Initialise(hitBeat, lane, word);
        }

        private static void ValidateLane(int lane)
        {
            if (lane < 0 || lane >= 3)
            {
                throw new Exception($"Lane {lane} out of range");
            }
           
        }

        private void ParseAbsolute(string line, string[] segments)
        {

            if (segments.Length < 3)
            {
                throw new Exception("Not enough segments, possibly missing comma");
            }

            string hitbeatString = segments[1]; //40
            string laneString = segments[2];//1

            string temp1 = line.Remove(0, line.IndexOf(',') + 1);//1,hello
            string temp2 = temp1.Remove(0, line.IndexOf(',') + 1);//1,hello                               
            string word = temp2.Remove(0, temp1.IndexOf(',') + 1);//hello

            word = word.Replace("\r", "");

            if(word.Length == 0 || word == "\n")
            {
                throw new Exception("No text in section");
            }


            notes.Add(new Note());
            float hitBeat;
            int lane;
            try
            {
                hitBeat = float.Parse(hitbeatString);
                lane = int.Parse(laneString);
            }
            catch
            {
                string possibleError = "";
                if (hitbeatString == ">")
                {
                    possibleError = " did you try to make a pass command absolute?";
                }
                Debug.LogError("Attempted to parse \"" + hitbeatString + "\" as a climax beat." + possibleError);
                throw;
            }
            ValidateLane(lane);

            notes[notes.Count - 1].Initialise(hitBeat, lane, word);

        }

        private void ParseEndSection(string line, string[] segments)
        {
           
            string onbeatString = segments[1];
            float onBeat;
            try
            {
                onBeat = float.Parse(onbeatString);
            }
            catch
            {
                Debug.LogError("Could not parse floats in end section");
                throw;
            }
            otherCommands.Add(new OtherCommand());
            otherCommands[otherCommands.Count - 1].Initialise(onBeat, CommandType.EndSection);
        }

        private void ParsePassToDialogue(string line, string[] segments)
        {
          
            if(segments.Length < 3)
            {
                throw new Exception("Not enough segments, possibly missing comma");
            }

            string passBeatString = segments[1];
            string returnBeatString = segments[2];
            float passBeat;
            float returnBeat;
            try
            {
                passBeat = float.Parse(passBeatString);
                returnBeat = float.Parse(returnBeatString);
            }
            catch
            {
                Debug.LogError("Could not parse floats in pass to dialogue");
                throw;
            }
            toDialogues.Add(new PassToDialogue());
            toDialogues[toDialogues.Count - 1].Initialise(passBeat, returnBeat);
        }
        //splits the currently loaded notesection into strings representing the individual lines said by the character ready to be previewed
        List<string> SplitSectionIntoStringLines(List<Note> notes, List<PassToDialogue> passes, List<OtherCommand> commands)
        {
            List<string> secLines = new List<string> { };
            List<float> lineEnds = new List<float> { };
            foreach (PassToDialogue pass in passes)
            {
                lineEnds.Add(pass.returnBeat);          //>,1,2
            }
            foreach (OtherCommand command in commands)
            {
                if (command.type == CommandType.EndSection)
                {
                    lineEnds.Add(command.onBeat);
                }
            }
            var unsortedLines = new List<float> (lineEnds);
            lineEnds.Sort();
            if (!Enumerable.SequenceEqual(lineEnds, unsortedLines)){
                Debug.LogError("sortedlistaaaaaaaaaaaaaaa");

            }
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
                        i = lineEnds.Count; //break - this is a break
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
                if (noteLines[i].Count > 0)
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

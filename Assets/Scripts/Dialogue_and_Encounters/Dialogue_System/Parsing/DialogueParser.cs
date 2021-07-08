using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System;
using System.Linq;

namespace Dialogue
{
    public class DialogueParser
    {
       
        public Conversation TryParse(string text)
        {

            var conversation = ScriptableObject.CreateInstance<Conversation>();

            string[] allLines = text.Split('\n');


            string metadataLine = allLines[0];

            var metadata = ParseMetadata(metadataLine);

            conversation.conversationID = metadata["id"].Value;

            var mode = GetMode(metadata["mode"].Value);

            conversation.initialMode = mode != DialogueMode.None ? mode : DialogueMode.Normal;


            string[] lines = allLines.Skip(1).Take(allLines.Length).ToArray(); // skip metadata line



            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                var header = ParseHeader(line, lineNumber: i);

                if (header == null)
                {
                    break;
                }

                ParseLineBody(conversation, i, line, header);
            }

            return conversation;


        }

        private void ParseLineBody(Conversation conversation, int i, string line, GroupCollection header)
        {
            string body = ParseBody(line, lineNumber: i + 1);

            var phrase = new DialoguePhrase();

            if (body.Contains('['))
            {
                body = ParseInlineInstructions(conversation, phrase, body, i, header);
            }

            string speaker = header["name"].Value;


            phrase.PhraseID = conversation.conversationID +"."+ i.ToString();
            phrase.Speaker = speaker;
            phrase.Phrase = new System.Text.StringBuilder(body);



            SetActions(header, phrase, conversation);

            conversation.dialoguePhrases.Add(phrase);
        }

    

        private void SetActions(GroupCollection header, DialoguePhrase phrase, Conversation conversation)
        {
            DialogueMode dialogueMode = GetMode(header["mode"].Value);
            if (dialogueMode != DialogueMode.None)
            {
                phrase.onTrigger += () => conversation.SetDialougeMode(dialogueMode);
            }
        }

        private DialogueMode GetMode(string value)
        {
            if(value == "")
            {
                return DialogueMode.None;
            }

            return (DialogueMode)int.Parse(value);  
           
        }

        private GroupCollection ParseMetadata(string line)
        {

            Match group = Regex.Match(line, @"^\s*id: (?<id>[ _\w]+)(|, mode: (?<mode>[\d]+))\s?$");

            if (!group.Success)
            {
                throw new System.Exception("Metadata malformed");
            }

            if (!group.Groups["id"].Success)
            {
                throw new System.Exception("Does not have id");
            }

            string value = group.Groups["mode"].Value;
            if (!group.Groups["mode"].Success && value != "")
            {
                throw new System.Exception("Metadata mode parsed incorectly");
            }

            return group.Groups;

        }

        private GroupCollection ParseHeader(string line, int lineNumber)
        {
            if(line == "")
            {
                return null;
            }

            if (Regex.IsMatch(line, @"^\[end\].*"))
            {
                return null;
            }

            Match headerGroup = Regex.Match(line, @"^\s*\[(?<name>[ \w]+)(|, mode: (?<mode>[ \w]+))\]: ");

            if (!headerGroup.Success)
            {
                throw new System.Exception($"Phrase {lineNumber} instructions malformed");
            }

            if (!headerGroup.Groups["name"].Success)
            {
                throw new System.Exception($"Phrase {lineNumber} does not have name");
            }

            string value = headerGroup.Groups["mode"].Value;
            if (!headerGroup.Groups["mode"].Success && value != "")
            {
                throw new System.Exception($"Phrase {lineNumber} mode parsed incorectly");
            }

            return headerGroup.Groups;

        }


        private string ParseBody(string line, int lineNumber, string header = null)
        {
            if(header == null)
            {
                header = ParseHeader(line, lineNumber)[0].Value;
            }

            line = line.Substring(header.Length);

            Match bodyGroup = Regex.Match(line, @"^(?<body>[ \w',\.\?\!<>=""\/\(\)\[\]\#:]+)\r$");  

            if (!bodyGroup.Success)
            {
                throw new System.Exception($"Phrase {lineNumber} body malformed");
            }

            if (!bodyGroup.Groups["body"].Success)
            {
                throw new System.Exception($"Phrase {lineNumber} body cannot be parsed");
            }

            string body = bodyGroup.Groups["body"].Value;
           


            return body;
        }


        // public readonly string[] instructions = { "mode", "colour" };

        enum Instructions
        {
            debug
            ,mode
            ,colour
            ,rythm
            ,pause
            ,shake
        }

        private string ParseInlineInstructions(Conversation conversation, DialoguePhrase phrase, string body, int lineNumber, GroupCollection header)
        {
            if (body.Count((c) => c == '[') != body.Count((c) => c == ']'))
            {
                throw new Exception($"Phrase {lineNumber} inline instructions malformed, open close missmatch");
            }

            MatchCollection instructionsGroups = Regex.Matches(body, @"\[([ #\w\:]+(?:.\d+)?)\]");


            int matchIndex = 0;
            foreach (Match match in instructionsGroups)
            {
                if (!match.Success)
                {
                    throw new Exception($"Phrase {lineNumber} instruction group {matchIndex} is malformed");
                }


                bool found = false;
                foreach (var iName in Helper.Utility.GetEnumValues<Instructions>())
                {
                    var instruction = Regex.Match(match.Value, $@"\[({iName}: (?<{iName}>[#\w]+(?:.\d+)?))\]");

                    if (!instruction.Success)
                    {
                        continue;
                    }

                    if (instruction.Groups[iName.ToString()].Value != "")
                    {
                        Action instructionAction = GetInstruction(conversation, iName, instruction);
                        phrase.AddInstruction(matchIndex, instructionAction);
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    throw new Exception($"Phrase {lineNumber} instruction group {matchIndex} cannot be parsed");
                }

                var escapedPattern = new Regex(Regex.Escape(match.Value));
                body = escapedPattern.Replace(body, $"[{matchIndex}]", 1);

                matchIndex++;
            } 

            return body;

        }

      

        private Action GetInstruction(Conversation conversation, Instructions iName, Match instruction)
        {
            string value = instruction.Groups[iName.ToString()].Value;

            switch (iName)
            {
                case Instructions.mode:
                    {
                        var v = int.Parse(value);
                        Action changeMode = () => conversation.SetDialougeMode((DialogueMode)v);
                        return changeMode;
                    }

                case Instructions.colour:
                    {
                        var v = int.Parse(value, System.Globalization.NumberStyles.HexNumber);
                        Action changeColour = () => conversation.SetColour(v);
                        return changeColour;
                    }

                case Instructions.debug:
                    {
                        Action changeMode = () => Debug.Log($"Inline debug: {value}. Conversation: {conversation.conversationID}");
                        return changeMode;
                    }

                case Instructions.rythm:
                    {
                        Action startRyhtmSection = () => conversation.StartRyhtmSection(value);

                        return startRyhtmSection;
                    }

                case Instructions.pause:
                    {
                        var v = int.Parse(value);
                        Action pause = () => conversation.Pause(v);
                        return pause;
                    }

                case Instructions.shake:
                    {
                        var v = float.Parse(value);
                        Action shake = () => conversation.Shake(v);

                        return shake;
                    }
                default: throw new Exception("Instruction name exists but no code handles this case");
            }

        }
    }
}
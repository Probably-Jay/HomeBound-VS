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
        //public Conversation Parse(string text)
        //{
        //    try
        //    {

        //        return TryParse(text);
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }

        //}

        public Conversation TryParse(string text)
        {

            var conversation = ScriptableObject.CreateInstance<Conversation>();

            string[] allLines = text.Split('\n');


            string metadataLine = allLines[0];

            var metadata = ParseMetadata(metadataLine);

            conversation.conversationID = metadata["id"].Value;

            var mode = GetMode(metadata["mode"].Value);

            conversation.initialMode = mode != DialogueMode.None ? mode : DialogueMode.Normal;


            string[] lines = allLines.Skip(1).Take(allLines.Length).ToArray();


            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                var header = ParseHeader(line,lineNumber: i); 

                if (header == null)
                {
                    break;
                }

                string body = ParseBody(line, lineNumber: i);

                string speaker = header["name"].Value;

                var phrase = new DialoguePhrase(body, speaker);

                SetActions(header, phrase, conversation);

                conversation.dialoguePhrases.Add(phrase);
            }

            return conversation;


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
            switch (value)
            {
                case "":
                    return DialogueMode.None;
                case "0":
                    return DialogueMode.Normal;
                case "1":
                    return DialogueMode.Encounter_OpponentSpeak;
                case "2":
                    return DialogueMode.Encounter_PlayerSpeak;
                default:
                    throw new Exception("Mode provided not valid");
            }
        }

        private GroupCollection ParseMetadata(string line)
        {

            Match group = Regex.Match(line, @"^\s*id: (?<id>[ \w]+)(|, mode: (?<mode>[ \w]+))\s?$");

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

            // todo find \r manually and remove it

            Match bodyGroup = Regex.Match(line, @"^[ \w',\.\?\!<>=""/]+"); // this will fail silently if does not reach end

            if (!bodyGroup.Success)
            {
                throw new System.Exception($"Phrase {lineNumber} body malformed");
            }

            return bodyGroup.Value;
        }


    }
}
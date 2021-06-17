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
        public Conversation Parse(string text)
        {
            var conversation = ScriptableObject.CreateInstance<Conversation>();

            string[] allLines = text.Split('\n');


            string metadataLine = allLines[0];

            var metadata = ParseMetadata(metadataLine);

            conversation.conversationID = metadata[1].Value;

            var mode = GetMode(metadata[3].Value);

            conversation.dialogueMode = mode;
                

            string[] lines = allLines.Skip(1).Take(allLines.Length).ToArray();


            foreach (var line in lines)
            {
                var header = ParseHeader(line);

                if (header == null)
                {
                    break;
                }

                string body = ParseBody(line);

                string headerContent = header[1].Value;

                

                var phrase = new DialoguePhrase(body, headerContent);

                conversation.dialoguePhrases.Add(phrase);
            }

            return conversation;
        }

        private DialogueMode GetMode(string value)
        {
            switch (value)
            {
                case "":
                    return DialogueMode.Normal;
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
            Match group = Regex.Match(line, @"^\s*id: ([ \w]+)(|, mode: ([ \w]+))\s?$");

            if (!group.Success)
            {
                throw new System.Exception("Metadata malformed");
            }

            if (!group.Groups[1].Success)
            {
                throw new System.Exception("Does not have starting instructions");
            }

            return group.Groups;

        }

        public GroupCollection ParseHeader(string line)
        {
            if(line == "")
            {
                return null;
            }

            Match headerGroup = Regex.Match(line, @"^\s*\[([ \w]+)\]: ");

            if (!headerGroup.Success)
            {
                throw new System.Exception("Starting instructions malformed");
            }

            if (!headerGroup.Groups[1].Success)
            {
                throw new System.Exception("Does not have starting instructions");
            }

            return headerGroup.Groups;

        }


        private string ParseBody(string line, string header = null)
        {
            if(header == null)
            {
                header = ParseHeader(line)[0].Value;
            }

            line = line.Substring(header.Length);

            // todo find \r manually and remove it

            Match bodyGroup = Regex.Match(line, @"^[ \w]+"); // this will fail silently if does not reach end

            if (!bodyGroup.Success)
            {
                throw new System.Exception("body malformed");
            }

            return bodyGroup.Value;
        }


    }
}
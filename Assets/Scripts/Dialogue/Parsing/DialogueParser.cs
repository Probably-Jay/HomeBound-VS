using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System;

namespace Dialogue
{
    public class DialogueParser
    {
        public Conversation Parse(string text)
        {
            string[] lines = text.Split('\n');

            var conversation = ScriptableObject.CreateInstance<Conversation>();
       

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


        public GroupCollection ParseHeader(string line)
        {
            if(line == "")
            {
                return null;
            }

            Match headerGroup = Regex.Match(line, @"^\s*\[([\w\s]+)\]:\s");     

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


            Match bodyGroup = Regex.Match(line, @"^[ \w]+");

            if (!bodyGroup.Success)
            {
                throw new System.Exception("body malformed");
            }

            return bodyGroup.Value;
        }


    }
}
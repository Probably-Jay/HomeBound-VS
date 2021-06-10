using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dialogue;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Tests
{
    public class DialogueParsingSystemTests 
    {
        DialogueParser dialogueParser;
        DialogueLoader dialogueLoader;

        string rawTestText;

        const string TestName3 = "TestFile3";

        const string speaker1 = "person 1";
        const string body1 = "test 1";


        [SetUp]
        public void SetUp()
        {
            dialogueLoader = new DialogueLoader();
            dialogueLoader.Load(Game.TextAssetFolders.Test);

            rawTestText = dialogueLoader.GetRawData(Game.TextAssetFolders.Test, TestName3);

            dialogueParser = new DialogueParser();

        }

        [Test]
        public void ParseTestFileHeaderCorrectly()
        {
            var conversation = dialogueParser.Parse(rawTestText);

            var header = conversation.dialoguePhrases[0].Speaker;

            Assert.AreEqual(speaker1, header);
        }        
        
        [Test]
        public void ParseTestFileBodyCorrectly()
        {
            var conversation = dialogueParser.Parse(rawTestText);

            var body = conversation.dialoguePhrases[0].Phrase;

            Assert.AreEqual(body1, body);
        }

        [TearDown]
        public void CleanUp()
        {
            dialogueLoader = null;
            rawTestText = null;
            dialogueParser = null;
               
        }
    }
}
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
        const string TestName4 = "TestFile4";

        const string id1 = "test_3";

        const string speaker1 = "person 1";
        const string body1 = "test 1";

        const Dialogue.DialogueMode defaultMode = DialogueMode.Normal;
        const Dialogue.DialogueMode modeTest4 = DialogueMode.Encounter_OpponentSpeak;

        

        [SetUp]
        public void SetUp()
        {
            dialogueLoader = new DialogueLoader();

            dialogueLoader.Load(Game.TextAssetFolders.Test);


            dialogueParser = new DialogueParser();

        }

        [Test]
        public void ParseMetadataCorrectly()
        {
            rawTestText = dialogueLoader.GetRawData(Game.TextAssetFolders.Test, TestName3);


            var conversation = dialogueParser.TryParse(rawTestText);

            var id = conversation.conversationID;

            Assert.AreEqual(id1, id);
        }

        [Test]
        public void ParseTestFileHeaderSpeakerCorrectly()
        {
            rawTestText = dialogueLoader.GetRawData(Game.TextAssetFolders.Test, TestName3);


            var conversation = dialogueParser.TryParse(rawTestText);

            var speaker = conversation.dialoguePhrases[0].Speaker;

            Assert.AreEqual(speaker1, speaker);
        }        
        
        [Test]
        public void ParseTestFileHeaderDefaultModeCorrectly()
        {
            rawTestText = dialogueLoader.GetRawData(Game.TextAssetFolders.Test, TestName3);


            var conversation = dialogueParser.TryParse(rawTestText);

            var mode = conversation.initialMode;

            Assert.AreEqual(defaultMode, mode);
        }  
        
        [Test]
        public void ParseTestFileHeaderExplicitModeCorrectly()
        {
            rawTestText = dialogueLoader.GetRawData(Game.TextAssetFolders.Test, TestName4);


            var conversation = dialogueParser.TryParse(rawTestText);

            var mode = conversation.initialMode;

            Assert.AreEqual(modeTest4, mode);
        }        
        
        [Test]
        public void ParseTestFileBodyCorrectly()
        {
            rawTestText = dialogueLoader.GetRawData(Game.TextAssetFolders.Test, TestName3);


            var conversation = dialogueParser.TryParse(rawTestText);

            var body = conversation.dialoguePhrases[0].Phrase;

            Assert.AreEqual(body1, body);
        }

        [Test]
        public void ParseAll()
        {
            foreach (var folder in Helper.Utility.GetEnumValues<Game.TextAssetFolders>())
            {

                dialogueLoader.UnloadAll();

                if(folder == Game.TextAssetFolders.None)
                {
                    continue;
                }

                dialogueLoader.Load(folder);

                foreach (var conversationText in dialogueLoader.RawFileData)
                {
                    Conversation conversation;
                    try
                    {
                        conversation = dialogueParser.TryParse(conversationText.Value);

                    }
                    catch (System.Exception e)
                    {
                        throw new DialogueParsingException($"In folder: {folder}, file: {conversationText.Key}: {e.Message}");
                    }

                }
            }
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
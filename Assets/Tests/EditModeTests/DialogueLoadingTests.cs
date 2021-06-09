using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Dialogue;

namespace Tests
{
    public class DialogueLoadingTests
    {
        const string TestName1 = "TestFile1";
        const string TestName2 = "TestFile2";


        const string TestData1 = "This is test 1";
        const string TestData2 = "This is test 2";

        DialogueLoader dialogueLoader;

        // A Test behaves as an ordinary method
        [SetUp]
        public void SetUp()
        {
            // create loader
            dialogueLoader = new DialogueLoader();
            Assert.NotNull(dialogueLoader);
           
        }

        [Test]
        public void LoadTests()
        {
            dialogueLoader.Load(Game.TextAssetFolders.Test);
        }

        //[Test]
        //public void ValidateTestAssetsNames()
        //{
        //    dialogueLoader.Load(Game.TextAssetFolders.Test);

        //    Assert.IsTrue(dialogueLoader.RawFileData.ContainsKey(TestName1));
        //    Assert.IsTrue(dialogueLoader.RawFileData.ContainsKey(TestName2));
        //}       
        
        [Test]
        public void ValidateTestAssetsData()
        {
            dialogueLoader.Load(Game.TextAssetFolders.Test);

            Assert.AreEqual(dialogueLoader.GetRawData(Game.TextAssetFolders.Test,TestName1), TestData1);
            Assert.AreEqual(dialogueLoader.GetRawData(Game.TextAssetFolders.Test,TestName2), TestData2);
        }

        [TearDown]
        public void CleanUp()
        {
            dialogueLoader.UnloadAll();
        }


    }
}

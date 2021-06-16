using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Dialogue;
using System.IO;

namespace Tests
{
    public class DialogueLoadingSystemTests
    {
        const string TestName1 = "TestFile1";
        const string TestName2 = "TestFile2";

        const string TestData1 = "This is test 1";
        const string TestData2 = "This is test 2";

        DialogueLoader dialogueLoader;


        [SetUp]
        public void SetUp()
        {
            // create loader
            dialogueLoader = new DialogueLoader();
            Assert.NotNull(dialogueLoader);
        }

        [Test]
        public void DialogueAssetsLoad()
        {
            dialogueLoader.Load(Game.TextAssetFolders.Test);
            Assert.IsNotNull(dialogueLoader);
        }

        [Test]
        public void TestAssetsNamesAreValid()
        {
            dialogueLoader.Load(Game.TextAssetFolders.Test);

            Assert.IsTrue(dialogueLoader.RawFileData.ContainsKey(DialogueLoader.Combine(Game.TextAssetFolders.Test, TestName1)));
            Assert.IsTrue(dialogueLoader.RawFileData.ContainsKey(DialogueLoader.Combine(Game.TextAssetFolders.Test, TestName2)));
        }

        [Test]
        public void TestAssetsDataIsValid()
        {
            dialogueLoader.Load(Game.TextAssetFolders.Test);

            Assert.AreEqual(dialogueLoader.GetRawData(Game.TextAssetFolders.Test, TestName1), TestData1);
            Assert.AreEqual(dialogueLoader.GetRawData(Game.TextAssetFolders.Test, TestName2), TestData2);
        }

        [Test]
        public void AllCorrectDialogueFoldersExist()
        {
            var dirInfo = new DirectoryInfo(DialogueLoader.DialogueFilePath);
            int extantDirectoriesCount = dirInfo.GetDirectories().Length;
            int enumValueCount = Helper.Utility.GetEnumValues<Game.TextAssetFolders>().Length;
            Assert.AreEqual(extantDirectoriesCount, enumValueCount);
        }

        [Test]
        public void AllDialogueAssetsAreBeingLoaded()
        {
            int fileCount = 0;
            foreach (var folder in Helper.Utility.GetEnumValues<Game.TextAssetFolders>())
            {
                dialogueLoader.Load(folder);

                string path = DialogueLoader.GetFileDirectory(folder);

                fileCount += Directory.GetFiles(path, "*.txt").Length;
            }

            int loadedFileCount = dialogueLoader.RawFileData.Count;

            Assert.AreEqual(loadedFileCount, fileCount);
        }

        [TearDown]
        public void CleanUp()
        {
            dialogueLoader.UnloadAll();
            dialogueLoader = null;
        }

    }


}

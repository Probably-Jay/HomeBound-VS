using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;
using System.IO;

namespace Dialogue
{
    public class DialogueLoader
    {
        public string ResourcesPath => Path.Combine("Assets", "Resources");
        public string DialogueFilePath => Path.Combine(ResourcesPath, "DialogueFiles");

        private Dictionary<string, string> RawFileData = new Dictionary<string, string>();

        public string GetRawData(TextAssetFolders folder, string file) => RawFileData[Combine(folder.ToString(), file)];

        public void UnloadAll()
        {
            RawFileData = new Dictionary<string, string>();
        }

        public void Load(Game.TextAssetFolders folder)
        {
            string folderName = folder.ToString();

            string path = GetFileDirectory(folderName);


            foreach (string filePath in Directory.EnumerateFiles(path, "*.txt"))
            {
                string fileName;

                fileName = GetFileName(filePath);
                fileName = Combine(folderName, fileName);

                string contents = File.ReadAllText(filePath);

                RawFileData.Add(fileName, contents);
            }

        }

        private static string Combine(string folderName, string fileName) => $"{folderName}_{fileName}";

        private static string GetFileName(string filePath)
        {
            var begin = filePath.LastIndexOf(Path.DirectorySeparatorChar);
            var end = filePath.LastIndexOf('.');

            return filePath.Substring(begin + 1, (end - begin) -1);
        }

        private string GetFileDirectory(string folder) => Path.Combine(DialogueFilePath, folder);
    }
}
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
        public static string ResourcesPath => Path.Combine("Assets", "Resources");
        public static string DialogueFilePath => Path.Combine(ResourcesPath, "DialogueFiles");

        public readonly Dictionary<string, string> RawFileData = new Dictionary<string, string>();

        public string GetRawData(TextAssetFolders folder, string file) => RawFileData[Combine(folder, file)];

        public void UnloadAll()
        {
            RawFileData.Clear();
        }

        public void Unload(string folder)
        {
            if (!RawFileData.ContainsKey(folder))
            {
                return;
            }
            RawFileData.Remove(folder);
        }

        public void Load(Game.TextAssetFolders folder)
        {

            string path = GetFileDirectory(folder);


            foreach (string filePath in Directory.EnumerateFiles(path, "*.txt"))
            {
                string fileName;

                fileName = GetFileName(filePath);
                fileName = Combine(folder, fileName);

                string contents = File.ReadAllText(filePath);

                RawFileData.Add(fileName, contents);
            }

        }

        //public void Load(TextAssetFolders textAssetFolders, object folder)
        //{
        //    throw new NotImplementedException();
        //}

        public static string Combine(Game.TextAssetFolders folderName, string fileName) => $"{folderName}_{fileName}";

        private static string GetFileName(string filePath)
        {
            var begin = filePath.LastIndexOf(Path.DirectorySeparatorChar);
            var end = filePath.LastIndexOf('.');

            return filePath.Substring(begin + 1, (end - begin) -1);
        }

        public static string GetFileDirectory(Game.TextAssetFolders folder) => Path.Combine(DialogueFilePath, folder.ToString());
    }
}
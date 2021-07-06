using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;

namespace Dialogue
{
    public class DialogueLoader
    {
        //  public static string ResourcesPath => Path.Combine("Assets", "Resources");
        public static string StreamingPath
        {
            get
            {
                string streamingAssetsPath;
                try
                {
                    streamingAssetsPath = Application.streamingAssetsPath;

                }
                catch (Exception e)
                {

                    throw new Exception("Streaming assets unavalibale");
                }
                return streamingAssetsPath;
            }
        }

        public static string DialogueFilePath => Path.Combine(StreamingPath, "DialogueFiles");

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

                string contents = GetData(filePath,fileName);



                RawFileData.Add(fileName, contents);
            }

        }

        private string GetData(string path,string fileName)
        {

            string data;

            if (Application.platform == RuntimePlatform.WebGLPlayer || true)
            {
                UnityWebRequest request = UnityWebRequest.Get(path);
                request.SendWebRequest();

                while (!request.isDone)
                {
                    if (request.isNetworkError || request.isHttpError)
                    {
                        throw new Exception("Can't get file");
                       // break;
                    }
                }

                byte[] rawData = request.downloadHandler.data;
                if (request.isNetworkError || request.isHttpError || rawData == null)
                {
                    Debug.LogError("Networking error");
                    return "There was a web-error getting all of the dialogue... all text will say this, sorry. Please inform a member of our team, try re-loading the page?";
                }

                //string cachePath = Path.Combine(Application.temporaryCachePath, $"temp_{fileName}.locDat");
                //File.WriteAllBytes(cachePath, rawData);
                //data = File.ReadAllText(cachePath, System.Text.Encoding.UTF8);

                data = System.Text.Encoding.UTF8.GetString(rawData);

            }
            //else
            //{
            //    data = File.ReadAllText(path, System.Text.Encoding.UTF8);

            //}

            return data;
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

            return filePath.Substring(begin + 1, (end - begin) - 1);
        }

        public static string GetFileDirectory(Game.TextAssetFolders folder) => Path.Combine(DialogueFilePath, folder.ToString());
    }
}
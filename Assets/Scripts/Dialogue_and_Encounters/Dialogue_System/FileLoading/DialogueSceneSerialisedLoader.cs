using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;

namespace Dialogue
{
    public class DialogueSceneSerialisedLoader : MonoBehaviour, IDialogueLoader
    {
        Dictionary<TextAssetFolders, List<TextAsset>> virtualFolders = new Dictionary<TextAssetFolders, List<TextAsset>>();

        [SerializeField] List<TextAsset> test;
        public event Action OnLoad;

        private void Awake()
        {
            virtualFolders.Add(TextAssetFolders.Main, test);
        }



        public Dictionary<string, string> RawFileData { get; } = new Dictionary<string, string>();

        public void Load(TextAssetFolders folder)
        {
            if (!virtualFolders.ContainsKey(folder))
            {
                throw new System.Exception($"Folder {folder} has not been added to {nameof(virtualFolders)}");
            }

            foreach (var asset in virtualFolders[folder])
            {
                RawFileData.Add(asset.name, asset.text);
            }

            OnLoad?.Invoke();
        }

        public void Register(ISimpleDialogue text)
        {
            RawFileData.Add(text.Title, text.Text);
        }

        public void Unload(string folder)
        {
            if (!RawFileData.ContainsKey(folder))
            {
                Debug.LogError($"Folder {folder} cannot be unloaded as is not loaded");
                return;
            }
            RawFileData.Remove(folder);
        }

        public void UnloadAll()
        {
            RawFileData.Clear();
        }
    }
}
using System.Collections.Generic;

namespace Dialogue
{
    public interface IDialogueLoader
    {
        void UnloadAll();
        void Unload(string folder);

        void Load(Game.TextAssetFolders folder);

        /// <summary>
        /// The data in files of currentl loaded folder, adressable by filename
        /// </summary>
        Dictionary<string, string> RawFileData { get; }
    }
}
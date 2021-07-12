using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;

namespace Dialogue {

    public class ConversationHandler : MonoBehaviour
    {
        IDialogueLoader dialogueLoader;
        DialogueParser dialogueParser;

        //   readonly List<Game.TextAssetFolders> loadedConversations;

        public Dictionary<string, Conversation> Conversations { get; } = new Dictionary<string, Conversation>();

        public TextAssetFolders LoadedConversationFolder { get; private set; } = TextAssetFolders.None;


        private void Awake()
        {
            dialogueLoader = FindObjectOfType<DialogueSceneSerialisedLoader>();
            if(dialogueLoader == null)
            {
                Debug.LogError($"Cannot find {nameof(DialogueSceneSerialisedLoader)}");
            }
            dialogueParser = new DialogueParser();
        }

        public void UnLoad()
        {
            Conversations.Clear();
            LoadedConversationFolder = TextAssetFolders.None;
            dialogueLoader.UnloadAll();
        }

        public void Load(TextAssetFolders folder)
        {
            if (LoadedConversationFolder == folder)
                return;

            UnLoad();

            dialogueLoader.Load(folder);

            foreach (var conversationText in dialogueLoader.RawFileData)
            {
                Conversation conversation;
                try
                {
                    conversation = dialogueParser.TryParse(conversationText.Value);
                }
                catch 
                {
                    Debug.LogError($"Parsing error in folder: {folder}, file: {conversationText.Key}");
                    throw;
                }

                Conversations.Add(conversation.conversationID, conversation);
            }

            LoadedConversationFolder = folder;
        }
       
    }
}
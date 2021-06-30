using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;

namespace Dialogue {

    public class ConversationHandler : MonoBehaviour
    {
        DialogueLoader dialogueLoader;
        DialogueParser dialogueParser;

        readonly Dictionary<string, Conversation> conversations = new Dictionary<string, Conversation>();
     //   readonly List<Game.TextAssetFolders> loadedConversations;

        public Dictionary<string, Conversation> Conversations { get => conversations; }

        public TextAssetFolders LoadedConversationFolder { get; private set; } = TextAssetFolders.None;


        private void Awake()
        {
            dialogueLoader = new DialogueLoader();
            dialogueParser = new DialogueParser();
        }

        public void UnLoad()
        {
            conversations.Clear();
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
                catch (System.Exception e)
                {
                    throw new DialogueParsingException($"In folder: {folder}, file: {conversationText.Key}: {e.Message}: {e.StackTrace}");
                }

                Conversations.Add(conversation.conversationID, conversation);
            }

            LoadedConversationFolder = folder;
        }
       
    }
}
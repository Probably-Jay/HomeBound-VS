using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dialogue
{
    [RequireComponent(typeof(ConversationHandler),typeof(DialogueContextController))]
    public class DialogueManager : MonoBehaviour
    {

        ConversationHandler conversationHandler;
        DialogueContextController dialogueContextController;
        private Coroutine currentConversation;

        public event Action OnQueueDepleated;

        private void Awake()
        {

            conversationHandler = GetComponent<ConversationHandler>();
            dialogueContextController = GetComponent<DialogueContextController>();

            
            
        }

        private void OnEnable()
        {
            dialogueContextController.OnReachedEndOfQueue += InvokeQueuDepleated;
        }

        private void OnDisable()
        {
            dialogueContextController.OnReachedEndOfQueue -= InvokeQueuDepleated;
        }
        void InvokeQueuDepleated() => OnQueueDepleated?.Invoke();

        public void Load(Game.TextAssetFolders folder)
        {
            if(folder == Game.TextAssetFolders.None)
            {
                throw new ArgumentException($"Cannot load {Game.TextAssetFolders.None}");
            }
            conversationHandler.Load(folder);
        }

        public void BeginConversation(string conversationID)
        {
            if (!conversationHandler.Conversations.ContainsKey(conversationID))
            {
                throw new Exception($"Conversation {conversationID} does not exist or is not currently loaded in {conversationHandler.LoadedConversationFolder}");
            }

            var conversation = conversationHandler.Conversations[conversationID];

            StopCurrentConversation();

            StartNewConversation(conversation);


             
        }

        private void StartNewConversation(Conversation conversation) 
        {
            dialogueContextController.SetDialougeMode(conversation.initialMode);
            dialogueContextController.StartNewConversation();
            currentConversation = StartCoroutine(QueueConversation(conversation));

        }

        private void StopCurrentConversation()
        {
            if(currentConversation != null)
                StopCoroutine(currentConversation);
            dialogueContextController.StopConversation();
        }



        private IEnumerator QueueConversation(Conversation conversation)
        {
            conversation.OnSetDialogueMode += (mode) => dialogueContextController.SetDialougeMode(mode);

            foreach (DialoguePhrase phrase in conversation.dialoguePhrases)
            {
                 
                dialogueContextController.QueuePhrase(phrase);
            }

            yield break;
        }
    }
}
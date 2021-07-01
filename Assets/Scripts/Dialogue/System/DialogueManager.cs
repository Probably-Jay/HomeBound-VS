using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dialogue
{
    [RequireComponent(typeof(ConversationHandler),typeof(DialogueContextController),typeof(RythmDialogueInterface))]
    public class DialogueManager : MonoBehaviour
    {

        ConversationHandler conversationHandler;
        DialogueContextController dialogueContextController;
        RythmDialogueInterface rythmInterface;

        private Coroutine currentConversation;

        public event Action OnQueueDepleated;

        public bool InRythmSection => rythmInterface.InRythmSection;

        private void Awake()
        {
            conversationHandler = GetComponent<ConversationHandler>();
            dialogueContextController = GetComponent<DialogueContextController>();
            rythmInterface = GetComponent<RythmDialogueInterface>();
        }

        /// <summary>
        /// Stop the conversation and close the box
        /// </summary>
        public void Close()
        {
            StopCurrentConversation();
            transform.parent.gameObject.SetActive(false);
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
            Debug.Log($"StartingConversation { conversationID}");

            AssertContainsConversation(conversationID);

            var conversation = conversationHandler.Conversations[conversationID];

            StopCurrentConversation();

            StartNewConversation(conversation);



        }

        public void AssertContainsConversation(string conversationID)
        {
            if (!ContainsConversation(conversationID))
            {
                throw new Exception($"Conversation {conversationID} does not exist or is not currently loaded in {conversationHandler.LoadedConversationFolder}");
            }
        }

        public bool ContainsConversation(string conversationID) => conversationHandler.Conversations.ContainsKey(conversationID);

        private void StartNewConversation(Conversation conversation) 
        {
            dialogueContextController.SetDialougeMode(conversation.initialMode);
            dialogueContextController.StartNewConversation();
            currentConversation = StartCoroutine(QueueConversation(conversation));

        }

        internal void StopCurrentConversation()
        {
            //InRythmSection = false;

            if(currentConversation != null)
                StopCoroutine(currentConversation);
            dialogueContextController.StopConversation();
        }



        private IEnumerator QueueConversation(Conversation conversation)
        {
            conversation.OnSetDialogueMode += (mode) => dialogueContextController.SetDialougeMode(mode); // add changing mode events
            conversation.OnSetColour += (colour) => dialogueContextController.AddColourRTT(colour);
            conversation.OnTriggerRythmSection += (id) => rythmInterface.StartNewRythm(id);

            foreach (DialoguePhrase phrase in conversation.dialoguePhrases)
            {
                if (phrase.Queued)
                {
                    Debug.LogWarning($"You may be queueing this dialogue more than once in {conversation.conversationID}");
                }
                dialogueContextController.QueuePhrase(phrase);
            }

            yield break;
        }


        /// <summary>
        /// Start new rythm section
        /// </summary>
        //public void EnterRythmEncounter(string id)
        //{
        //    StopCurrentConversation();
        //    dialogueContextController.EnterArgument();
        //    InRythmSection = true;

        //    throw new NotImplementedException("Ryhtm start not implimented");

        //}

        /// <summary>
        /// Stop a rythm section
        /// </summary>
        //public void ExitRythmEncounter()
        //{
        //    StopCurrentConversation();
        //    InRythmSection = false;
        //}


        // These functions are intented to be used in rythm sections only

        /// <summary>
        /// Clear the currently shown text and progress to the next empty page. May only be called in rythm sections
        /// </summary>
        /// <param name="speaker">The new speaker of the dialogue</param>
        /// <param name="newDialogueMode">The new mode by which the text will be typed out. <see cref="Dialogue.DialogueMode.None"/> will leave this unchanged</param>
        /// <param name="onBeat">The beat this will occur on, leave <c>null</c> to trigger immidiatley</param>
        /// <param name="forceContext">Experimental, will prevent this from triggering unexpectely far in the future</param>
        public void ProgressNewPhraseDirectly(string speaker, DialogueMode newDialogueMode = DialogueMode.None, float? onBeat = null, bool forceContext = false)
        {
            AssertInRythmSection();
            SetTypingMode(newDialogueMode);
            dialogueContextController.ProgressNewPhraseDirectly(speaker, onBeat, forceContext);
        }

        /// <summary>
        /// Set the output mode. <see cref="Dialogue.DialogueMode.None"/> will leave the mode unchanged
        /// </summary>
        /// <param name="newDialogueMode">The new mode</param>
        public void SetTypingMode(DialogueMode newDialogueMode)
        {
            if (newDialogueMode == DialogueMode.None)
            {
                return;
            }
            dialogueContextController.SetDialougeMode(newDialogueMode);
        }

        /// <summary>
        /// Add a string to the current output. May only be called in rythm sections
        /// </summary>
        /// <param name="text">The text to display</param>
        /// <param name="onBeat">The beat this will occur on, leave <c>null</c> to trigger immidiatley</param>
        /// <param name="forceContext">Experimental, will prevent this from triggering unexpectely far in the future</param>
        public void AddWordDirectly(string text, float? onBeat = null, bool forceContext = false)
        {
            AssertInRythmSection();
            dialogueContextController.AddWordDirectly(text, onBeat, forceContext);
        }

        private void AssertInRythmSection()
        {
            if (InRythmSection) return;
            throw new Exception("This function may only be called in a rythm section");
        }
    }
}
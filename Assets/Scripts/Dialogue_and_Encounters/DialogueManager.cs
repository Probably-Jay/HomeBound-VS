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
        //    RythmDialogueInterface rythmInterface;
        IRythmDialogeControlInterface rythmDialogeControlInterface;

        private Coroutine currentConversation;

        public event Action OnQueueDepleated;

        public bool InRythmSection => rythmDialogeControlInterface.InRythmSection;

        public bool ThisHasControl { get => InRythmSection ? HasControl : true; }
        public bool ReadyToPassToRythm { get; private set; }

        bool HasControl => !rythmDialogeControlInterface.RythmHasControl;

        private void Awake()
        {
            conversationHandler = GetComponent<ConversationHandler>();
            dialogueContextController = GetComponent<DialogueContextController>();
            rythmDialogeControlInterface = GetComponent<RythmDialogueInterface>();
        }



        /// <summary>
        /// Stop the conversation and close the box
        /// </summary>
        internal void Close()
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

        internal void Load(Game.TextAssetFolders folder)
        {
            if(folder == Game.TextAssetFolders.None)
            {
                throw new ArgumentException($"Cannot load {Game.TextAssetFolders.None}");
            }
            conversationHandler.Load(folder);
        }

        internal void BeginConversation(string conversationID)
        {

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
            dialogueContextController.ResetDialogeModeTo(conversation.initialMode);
            dialogueContextController.StartNewConversation();
            currentConversation = StartCoroutine(QueueConversation(conversation));

        }

        internal void StopCurrentConversation()
        {
            if(currentConversation != null)
                StopCoroutine(currentConversation);
            dialogueContextController.StopConversation();
        }



        private IEnumerator QueueConversation(Conversation conversation)
        {
            conversation.OnSetDialogueMode += (mode) =>
            {
                //if (mode == DialogueMode.None)
                //{
                //    dialogueContextController.ReturnToPreviousMode();
                //    return;
                //}
                dialogueContextController.MutateDialogeMode(mode); 
            }; 
            conversation.OnSetColour += (colour) => dialogueContextController.AddColourRTT(colour);
            conversation.OnTriggerRythmSection += (id) => rythmDialogeControlInterface.StartNewRythm(id);
            conversation.OnPause += (value) => dialogueContextController.PauseTyping(value);
            conversation.OnShake += (value) => dialogueContextController.Shake(value);


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

        internal void AddLinePreview(string line)
        {
            throw new NotImplementedException();
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

        internal void EnterArgument()
        {
            dialogueContextController.EnterArgument();
            dialogueContextController.OnTypedPhrase += ReleaseControl;
        }

        internal void RythmControlReceived()
        {
            ReadyToPassToRythm = false;
            dialogueContextController.ProgressArgument();
        }

        internal void RythmControlYeilded()
        {
            dialogueContextController.ClearBox();
            dialogueContextController.MutateDialogeMode(DialogueMode.Encounter_PlayerSpeak);
        }

        internal void ReleaseControl()
        {
            if (!HasControl)
            {
                return;
            }
            ReadyToPassToRythm = true;
            rythmDialogeControlInterface.PassControlToRythm();
        }

        internal void LeaveArgument()
        {
            dialogueContextController.OnTypedPhrase -= ReleaseControl;
            dialogueContextController.LeaveArgument();
        }

        /// <summary>
        /// Set the output mode. <see cref="Dialogue.DialogueMode.None"/> will leave the mode unchanged
        /// </summary>
        /// <param name="newDialogueMode">The new mode</param>
        private void SetTypingMode(DialogueMode newDialogueMode)
        {
            if (newDialogueMode == DialogueMode.None)
            {
                return;
            }
            dialogueContextController.MutateDialogeMode(newDialogueMode);
        }

        /// <summary>
        /// Clear the currently shown text and progress to the next empty page. May only be called in rythm sections
        /// </summary>
        /// <param name="speaker">The new speaker of the dialogue</param>
        /// <param name="newDialogueMode">The new mode by which the text will be typed out. <see cref="Dialogue.DialogueMode.None"/> will leave this unchanged</param>
        /// <param name="onBeat">The beat this will occur on, leave <c>null</c> to trigger immidiatley</param>
        /// <param name="forceContext">Experimental, will prevent this from triggering unexpectely far in the future</param>
        internal void ProgressNewPhraseDirectly(string speaker, DialogueMode newDialogueMode = DialogueMode.None, float? onBeat = null, bool forceContext = false)
        {
            AssertRythmSectionHasControl();
            SetTypingMode(newDialogueMode);
            dialogueContextController.ProgressNewPhraseDirectly(speaker, onBeat, forceContext);
        }

        /// <summary>
        /// Add a string to the current output. May only be called in rythm sections
        /// </summary>
        /// <param name="text">The text to display</param>
        /// <param name="onBeat">The beat this will occur on, leave <c>null</c> to trigger immidiatley</param>
        /// <param name="forceContext">Experimental, will prevent this from triggering unexpectely far in the future</param>
        internal void AddWordDirectly(string text, NoteSystem.HitQuality hitQuality, float? onBeat = null, bool forceContext = false)
        {
            AssertRythmSectionHasControl();
            dialogueContextController.AddWordDirectly(text, (Dialogue.HitQuality)(int)hitQuality, onBeat, forceContext);
        }

        private void AssertRythmSectionHasControl()
        {
            if (ThisHasControl) throw new Exception("This function may only be called in a rythm section");
        }
    }
}
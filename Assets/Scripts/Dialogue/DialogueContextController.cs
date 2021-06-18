using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dialogue
{
    public enum DialogueMode
    { 
        None
        ,Normal
        ,Encounter_OpponentSpeak
        ,Encounter_PlayerSpeak
    }

    [RequireComponent(typeof(DialogueTyper))]
    public class DialogueContextController : MonoBehaviour
    {
        DialogueTyper dialogueTyper;

        public event Action OnReachedEndOfQueue;

        [SerializeField] private DialogueMode dialogueMode;

        private void Awake()
        {
            dialogueTyper = GetComponent<DialogueTyper>();
           
        }

        private void OnEnable()
        {
            dialogueTyper.OnReachedEndOfQueue += InvokeReachedEndOfQueue;
        }

        private void OnDisable()
        {
            dialogueTyper.OnReachedEndOfQueue -= InvokeReachedEndOfQueue;
        }
    

        void InvokeReachedEndOfQueue() => OnReachedEndOfQueue?.Invoke();

        public void SetDialougeMode(DialogueMode mode)
        {
            dialogueMode = mode;
            switch (mode)
            {
                case DialogueMode.Normal:
                    dialogueTyper.OnBeat = false;
                    dialogueTyper.TypingMode = TypingMode.Character;
                    dialogueTyper.StandardTypingDelay = 0.04f;
                    dialogueTyper.RandomTypingDelayDelta = 0.02f;

                    break;
                case DialogueMode.Encounter_OpponentSpeak:
                    dialogueTyper.OnBeat = true;
                    dialogueTyper.TypingMode = TypingMode.WordByCharacter;
                    dialogueTyper.DisplayActionsPerBeat = 2;

                    break;
                case DialogueMode.Encounter_PlayerSpeak:
                    dialogueTyper.OnBeat = true;
                    dialogueTyper.TypingMode = TypingMode.WordByCharacter;
                    dialogueTyper.DisplayActionsPerBeat = 2;

                    break;
                default: throw new System.ArgumentException($"{mode} not handled dialogue mode");
            }
        }

        public void QueuePhrase(DialoguePhrase phrase, float? onBeat = null, bool forceContext = false) => dialogueTyper.QueueNewPhrase(phrase, onBeat, forceContext);


        public void EnterArgument()
        {
            SetDialougeMode(DialogueMode.Encounter_OpponentSpeak);
            dialogueTyper.StopCurrent();
            dialogueTyper.StartNewNormal();
        }

        public void StopConversation()
        {
            dialogueTyper.StopCurrent();
        }

        public void StartNewConversation()
        {
            dialogueTyper.StartNewNormal();
        }

        public void ProgressNewPhraseDirectly(string speaker, float? onBeat = null, bool forceContext = false) => dialogueTyper.ProgressNewPhraseDirectly(speaker, onBeat, forceContext);
        public void AddWordDirectly(string text, float? onBeat = null, bool forceContext = false) => dialogueTyper.AddNewWordDirectly(text, onBeat, forceContext);

        //// Start is called before the first frame update
        //void Start()
        //{
        //    SetDialougeMode(dialogueMode);
        //    foreach (var phrase in currentConversation.dialoguePhrases)
        //    {
        //        if (phrase.Trigger != null && phrase.Trigger != "")
        //        {
        //            HandleTrigger(phrase.Trigger);
        //            continue;
        //        }

        //        dialogueController.QueueNewPhrase(phrase);
        //    }
        //}

        //private void HandleTrigger(string trigger)
        //{
        //    switch (trigger)
        //    {
        //        case "EnterArgument":
        //            EnterArgument();
        //            break;

        //        default: throw new ArgumentException($"<{trigger}> is not recognised");
        //    }
        //}

    }
}
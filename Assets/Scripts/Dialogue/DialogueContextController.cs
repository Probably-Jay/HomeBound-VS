using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dialogue
{
    public enum DialogueMode
    { 
        Normal
        ,Encounter_OpponentSpeak
        ,Encounter_PlayerSpeak
    }

    [RequireComponent(typeof(DialogueController))]
    public class DialogueContextController : MonoBehaviour
    {
        DialogueController dialogueController;

        [SerializeField] Conversation currentConversation;

        [SerializeField] private DialogueMode dialogueMode;

        private void Awake()
        {
            dialogueController = GetComponent<DialogueController>();
        }

        public void SetDialougeMode(DialogueMode mode)
        {
            switch (mode)
            {
                case DialogueMode.Normal:
                    dialogueController.OnBeat = false;
                    dialogueController.TypingMode = TypingMode.Character;
                    dialogueController.StandardTypingDelay = 0.04f;
                    dialogueController.RandomTypingDelayDelta = 0.02f;

                    break;
                case DialogueMode.Encounter_OpponentSpeak:
                    dialogueController.OnBeat = true;
                    dialogueController.TypingMode = TypingMode.WordByCharacter;
                    dialogueController.DisplayActionsPerBeat = 2;

                    break;
                case DialogueMode.Encounter_PlayerSpeak:
                    dialogueController.OnBeat = true;
                    dialogueController.TypingMode = TypingMode.WordByCharacter;
                    dialogueController.DisplayActionsPerBeat = 2;

                    break;
                default: throw new System.ArgumentException();
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            SetDialougeMode(dialogueMode);
            foreach (var phrase in currentConversation.dialoguePhrases)
            {
                if (phrase.Trigger != null && phrase.Trigger != "")
                {
                    HandleTrigger(phrase.Trigger);
                    continue;
                }

                dialogueController.QueueNewPhrase(phrase);
            }
        }

        private void HandleTrigger(string trigger)
        {
            switch(trigger)
            {
                case "EnterArgument": 
                    EnterArgument();
                    break;

                default: throw new ArgumentException($"<{trigger}> is not recognised");
            }
        }

        private void EnterArgument()
        {
            SetDialougeMode(DialogueMode.Encounter_OpponentSpeak);
        }
    }
}
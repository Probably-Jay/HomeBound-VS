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
        ,Word
        ,Instant
        ,Encounter_OpponentSpeak
        ,Encounter_PlayerSpeak
    }

    public enum HitQuality
    {
        Miss
       , Early
       , Late
       , Good
       , Great
       , Perfect
    }

    [RequireComponent(typeof(DialogueTyper))]
    public class DialogueContextController : MonoBehaviour
    {
        DialogueTyper dialogueTyper;

        public event Action OnReachedEndOfQueue;
        public event Action OnTypedPhrase;

        CameraControl.TriggerScreenShake screenShakeTrigger;

        [SerializeField] private Stack<DialogueMode> dialogueMode = new Stack<DialogueMode>();

        private void Awake()
        {
            dialogueTyper = GetComponent<DialogueTyper>();
            screenShakeTrigger = GetComponent<CameraControl.TriggerScreenShake>();
            if(dialogueTyper == null || screenShakeTrigger == null)
            {
                Debug.LogError("Cannot find component");
            }
        }

        private void OnEnable()
        {
            dialogueTyper.OnReachedEndOfQueue += InvokeReachedEndOfQueue;
            dialogueTyper.OnTypedPhrase += InvokeTypedPhrase;
        }

        private void OnDisable()
        {
            dialogueTyper.OnReachedEndOfQueue -= InvokeReachedEndOfQueue;
            dialogueTyper.OnTypedPhrase -= InvokeTypedPhrase;
        }


        void InvokeReachedEndOfQueue() => OnReachedEndOfQueue?.Invoke();
        void InvokeTypedPhrase() => OnTypedPhrase?.Invoke();


        public void ResetDialogeModeTo(DialogueMode initialMode)
        {
            dialogueMode.Clear();
            PushDialogeMode(initialMode);
        }

        public void PushDialogeMode(DialogueMode mode)
        {
            dialogueMode.Push(mode);
            ApplyMode();
        }

        public void MutateDialogeMode(DialogueMode mode)
        {
            dialogueMode.Pop();
            PushDialogeMode(mode);
        }

        public void ReturnToPreviousMode()
        {
            if (dialogueMode.Count <= 1)
            {
                throw new Exception("Cannot return past initial mode");
            }
            dialogueMode.Pop();
            ApplyMode();
        }


        private void ApplyMode()
        {
            var mode = dialogueMode.Peek();
            switch (mode)
            {
                case DialogueMode.None:
                    Debug.LogWarning($"{nameof(PushDialogeMode)} or {nameof(ReturnToPreviousMode)} called with {DialogueMode.None}, no changes made");
                    break;

                case DialogueMode.Normal:
                    dialogueTyper.OnBeat = false;
                    dialogueTyper.TypingMode = TypingMode.Character;
                    dialogueTyper.StandardTypingDelay = 0.04f;
                    dialogueTyper.RandomTypingDelayDelta = 0.02f;
                    dialogueTyper.SetContinueDisplayShow(true);

                    break;
                case DialogueMode.Word:
                    dialogueTyper.OnBeat = false;
                    dialogueTyper.TypingMode = TypingMode.Word;
                    dialogueTyper.DisplayActionsPerBeat = 2;
                    dialogueTyper.SetContinueDisplayShow(true);

                    break;
                case DialogueMode.Instant:
                    dialogueTyper.OnBeat = false;
                    dialogueTyper.TypingMode = TypingMode.Instant;
                    dialogueTyper.DisplayActionsPerBeat = 1;
                    dialogueTyper.SetContinueDisplayShow(true);

                    break;
                case DialogueMode.Encounter_OpponentSpeak:
                    dialogueTyper.OnBeat = true;
                    dialogueTyper.TypingMode = TypingMode.WordByCharacter;
                    dialogueTyper.DisplayActionsPerBeat = 2;
                    dialogueTyper.SetContinueDisplayShow(false);

                    break;
                case DialogueMode.Encounter_PlayerSpeak:
                    dialogueTyper.OnBeat = true;
                    dialogueTyper.TypingMode = TypingMode.Instant;
                    dialogueTyper.DisplayActionsPerBeat = 2;
                    dialogueTyper.SetContinueDisplayShow(false);

                    break;
                default: throw new System.ArgumentException($"{mode} not handled dialogue mode");
            }
        }

       

        public void QueuePhrase(DialoguePhrase phrase, float? onBeat = null, bool forceContext = false) => dialogueTyper.QueueNewPhrase(phrase, onBeat, forceContext);

        public void Shake(float value)
        {
            screenShakeTrigger.TriggerImpulse(value);
        }

        public void PauseTyping(int value)
        {
            dialogueTyper.PauseTyping(value);
        }

        public void EnterArgument()
        {
            PushDialogeMode(DialogueMode.Encounter_OpponentSpeak);
            dialogueTyper.StartNewRythm(); 
        }

        public void ProgressArgument()
        {
            MutateDialogeMode(DialogueMode.Encounter_OpponentSpeak);
            dialogueTyper.ProgressRythm();
        }

       

        public void LeaveArgument()
        {
            ClearBox();
            ReturnToPreviousMode();
            dialogueTyper.ResumeNormal();
        }

        public void ClearBox()
        {
            dialogueTyper.ClearBox();
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
        public void AddWordDirectly(string text, HitQuality hitQuality, float? onBeat = null, bool forceContext = false) => dialogueTyper.AddNewWordDirectly(text, hitQuality, onBeat, forceContext);

        public void AddColourRTT(int colour)
        {
            string hex = colour.ToString("X6");
            string tag = $"<color=#{hex}>";
            dialogueTyper.AddRichTextTag(tag);
        }

       

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
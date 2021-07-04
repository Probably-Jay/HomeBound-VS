﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rythm;
using RhythmSectionLoading;
using System;

namespace Dialogue
{
    public enum Controler 
    {
        None,
        Rythm,
        Dialogue
    }


    public class RythmDialogueInterface : MonoBehaviour, IRythmDialogeControlInterface
    {
        public Controler InControl { get; private set; }
        DialogueManager dialogueManager;
        [SerializeField]RhythmSectionManager rythmManager;

        public bool InRythmSection { get; private set; } = false;

        bool passToRyrhmQueued = false;
        public bool RythmHasControl { get; private set; } = false;


        private void Awake()
        {
            dialogueManager = GetComponent<DialogueManager>();
        }


        public void StartNewRythm(string id)
        {
            Game.GameContextController.Instance.PushContext(Game.Context.Rythm);
            InRythmSection = true;
           // dialogueManager.StopCurrentConversation();
            dialogueManager.EnterArgument();
            PassControlToRythm();
            rythmManager.LoadSection(id);
            
        }

        public void PassControlToDialogue()
        {
            AssertInRythmSection();
            RythmHasControl = false;
            rythmManager.RythmControlYeilded();
            dialogueManager.RythmControlReceived();
        }

        public void PassControlToDialogue(float passBack)
        {
            if (dialogueManager.ThisHasControl)
            {
                Debug.LogError("Dialogue already has control");
            }
            PassControlToDialogue();
            QueuePassBackToRythm(passBack);
        }

        private void QueuePassBackToRythm(float passBack)
        {
            if (passToRyrhmQueued)
            {
                Debug.LogError("Already has pasback queued");
            }
            passToRyrhmQueued = true;
            Rythm.RythmEngine.Instance.QueueActionAtExplicitBeat(PassControlToRythmOnBeat, passBack);
        }

        private void PassControlToRythmOnBeat()
        {
            if (!dialogueManager.ReadyToPassToRythm)
            {
                Debug.LogError("Not ready to pass to rythm");
            }
            if (!passToRyrhmQueued)
            {
                Debug.LogError("This pass back was not queued!");
            }
            passToRyrhmQueued = false;
            PassControlToRythm();
        }


        public void PassControlToRythm()
        {
            if (passToRyrhmQueued)
            {
                // we have this pass back queued for the future
                Debug.Log("Pass back queued for the furure");
                return;
            }
            RythmHasControl = true;
            dialogueManager.RythmControlYeilded();
            rythmManager.RythmControlReceived();
        }

        public void EndRythmSection()
        {
            InRythmSection = false; 
            dialogueManager.LeaveArgument();
            Game.GameContextController.Instance.ReturnToPreviousContext();
        }

        private void AssertInRythmSection()
        {
            if (!InRythmSection) throw new System.Exception("This function may only be called in a rythm section");
        }
    }
}
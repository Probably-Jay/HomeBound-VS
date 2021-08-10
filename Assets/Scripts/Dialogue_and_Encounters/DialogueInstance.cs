﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interactables;
using System;

namespace Dialogue
{
    public class DialogueInstance : BaseDialogue
    {

        private bool HasBackupDialogue => backupDialogueIDs.Count > 0;
        public bool RunMainOnlyOnce { get => runMainOnlyOnce; set => runMainOnlyOnce = value; }
        private bool SeenAllMainDialogue => mainIDIndex >= mainDialogueIDs.Count;

        public event Action OnBegunLastMainMainDialogue;



        [SerializeField] private List<string> mainDialogueIDs;
        [SerializeField] private bool runMainOnlyOnce = false;
        [SerializeField] private List<string> backupDialogueIDs;
        [SerializeField] bool StressRelieving;
        int mainIDIndex;
        int backupIDIndex;

        private void Awake()
        {
            FindOpener();
            Debug.Assert(opener != null, $"{this.ToString()} cannot find type {nameof(DialogueBoxOpener)} within the scene");

            if (mainDialogueIDs.Count == 0)
            {
                Debug.LogError($"{this.ToString()} does not contain any dialogue.", this);
                throw new System.Exception($"{this.ToString()} does not contain any dialogue.");
            }

            if (!RunMainOnlyOnce && backupDialogueIDs.Count > 0)
            {
                Debug.LogError($"{this.ToString()} contains backup dialogue that can never be reached.", this);
                throw new System.Exception($"{this.ToString()} contains backup dialogue that can never be reached.");
            }
        }

        private void OnEnable()
        {
            opener.OnBoxClose += Opener_OnBoxClose;
            opener.OnBoxClose += ProgressMainId;
        }

        private void OnDisable()
        {
            opener.OnBoxClose -= ProgressMainId;
            opener.OnBoxClose -= Opener_OnBoxClose;
        }

      

        private void Start()
        {
            foreach (var id in mainDialogueIDs)
            {
                opener.DialogeBox.AssertContainsConversation(id);
            }
            foreach (var id in backupDialogueIDs)
            {
                opener.DialogeBox.AssertContainsConversation(id);
            }
        }

        protected override void TriggerDialogue()
        {
            if (Game.GameContextController.Instance.OverStressed &!StressRelieving)
            {
                StartDialogue("overstressed_start");
                return;
            }
            if (mainIDIndex < mainDialogueIDs.Count)
            {
                StartDialogue(mainDialogueIDs[mainIDIndex]);
                //mainIDIndex++;

                if (SeenAllMainDialogue)
                {
                    OnBegunLastMainMainDialogue?.Invoke();
                    if (RunMainOnlyOnce && !HasBackupDialogue) // prevent needing to press again to clear
                    {
                        DisconnectFromInteractTrigger();
                    }
                }
                return;
            }

            if (!RunMainOnlyOnce)
            {
                mainIDIndex = 0;
                TriggerDialogue();
                return;
            }

            if (HasBackupDialogue)
            {
                int index = backupIDIndex % backupDialogueIDs.Count;
                StartDialogue(backupDialogueIDs[index]);
                backupIDIndex++;
                return;
            }

            DisconnectFromInteractTrigger();
        }

        public void ClearMainDialogues()
        {
            mainDialogueIDs.Clear();
            ResetMainDialogueIndex();
        }
        public void ClearBackupDialogues()
        {
            backupDialogueIDs.Clear();
            ResetBackupDialogueIndex();
        }
        internal void ProgressMainId()
        {
            if (!Game.GameContextController.Instance.OverStressed)
            {
                mainIDIndex++;
            }
        }

        public void AddMainDialogueElement(string dialogeID) => mainDialogueIDs.Add(dialogeID);
        public void AddBackupDialogueElement(string dialogeID) => backupDialogueIDs.Add(dialogeID);

        public void SetMainDialogue(string dialogeID)
        {
            ClearMainDialogues();
            mainDialogueIDs = new List<string>() { dialogeID };
        }    
        public void SetMainDialogue(IEnumerable<string> dialogeIDs)
        {
            ClearMainDialogues();
            mainDialogueIDs = new List<string>(dialogeIDs);
        }             
        public void SetBackupDialogue(string dialogeID)
        {
            ClearBackupDialogues();
            backupDialogueIDs = new List<string>() { dialogeID };
        } 
        public void SetBackupDialogue(IEnumerable<string> dialogeIDs)
        {
            ClearBackupDialogues();
            backupDialogueIDs = new List<string>(dialogeIDs);
        }

        public void ResetMainDialogueIndex() => mainIDIndex = 0;
        public void ResetBackupDialogueIndex() => backupIDIndex = 0;

        public void AlterMainDialogueIndex(int delta)
        {
            var temp = mainIDIndex + delta;
            if (temp < 0 || temp >= mainDialogueIDs.Count)
            {
                throw new Exception($"Index set to {temp} is invalid");
            }
            mainIDIndex = temp;
        }        
        
        public void AlterBackupDialogueIndex(int delta)
        {
            var temp = mainIDIndex + delta;
            if (temp < 0 || temp >= mainDialogueIDs.Count)
            {
                throw new Exception($"Index set to {temp} is invalid");
            }
            backupIDIndex = temp;
        }

    }
}

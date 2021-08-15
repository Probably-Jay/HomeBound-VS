using System.Collections;
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


        public event Action OnEndedLastMainMainDialogue;



        [SerializeField] private bool startsEmpty;
        [SerializeField] private List<string> mainDialogueIDs;
        [SerializeField] private bool runMainOnlyOnce = false;
        [SerializeField] private List<string> backupDialogueIDs;
        [SerializeField] bool StressRelieving;
       /* [SerializeField] */ bool DisconnectWhenCompleted = true;
        int mainIDIndex;
        int backupIDIndex;

        private void Awake()
        {
            FindOpener();
            Debug.Assert(opener != null, $"{this.ToString()} cannot find type {nameof(DialogueBoxOpener)} within the scene");

            if (startsEmpty)
            {
                return;
            }

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
        }

        private void OnDisable()
        {
            opener.OnBoxClose -= Opener_OnBoxClose;
        }

        protected override void Opener_OnBoxClose()
        {
            if (triggeredAction)
            {
                base.Opener_OnBoxClose();
                if (mainIDIndex < mainDialogueIDs.Count)
                    ProgressMainId();
                else
                    ProgressBackupID();
            }
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
                ProgressBackupID();
                return;
            }

            if (DisconnectWhenCompleted)
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

        public void ProgressMainId()
        {
            if (!Game.GameContextController.Instance.OverStressed)
            {
                mainIDIndex++;
                if (SeenAllMainDialogue)
                {
                    OnEndedLastMainMainDialogue?.Invoke();
                    if (RunMainOnlyOnce && !HasBackupDialogue && DisconnectWhenCompleted) // prevent needing to press again to clear
                    {
                        DisconnectFromInteractTrigger();
                    }
                }
            }
        }

        private void ProgressBackupID()
        {
            backupIDIndex++;
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

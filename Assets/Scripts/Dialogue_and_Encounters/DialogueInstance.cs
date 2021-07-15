using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interactables;
using System;

namespace Dialogue
{
    public class DialogueInstance : MonoBehaviour, IInteractableTriggered
    {

        private bool HasBackupDialogue => backupDialogueIDs.Count > 0;
        public bool RunMainOnlyOnce { get => runMainOnlyOnce; set => runMainOnlyOnce = value; }
        private bool SeenAllMainDialogue => mainIDIndex >= mainDialogueIDs.Count;

        public event Action OnBegunLastMainMainDialogue;



        [SerializeField] private List<string> mainDialogueIDs;
        [SerializeField] private bool runMainOnlyOnce = false;
        [SerializeField] private List<string> backupDialogueIDs;
        DialogueBoxOpener opener;
        int mainIDIndex;
        int backupIDIndex;
        private bool triggeredAction;

        public event Action OnTriggered;
        public event Action OnPostTriggered;
        public event Action<MonoBehaviour> OnDisconnectFromInteractTrigger;

        private void Awake()
        {
            opener = FindObjectOfType<DialogueBoxOpener>();
            Debug.Assert(opener != null, $"{this.ToString()} cannot find type {nameof(DialogueBoxOpener)} within the scene");

            if(mainDialogueIDs.Count == 0)
            {
                Debug.LogError($"{this.ToString()} does not contain any dialogue.", this);
                throw new System.Exception($"{this.ToString()} does not contain any dialogue.");
            }

            if(!RunMainOnlyOnce && backupDialogueIDs.Count > 0)
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

        private void TriggerDialogue()
        {
            if (mainIDIndex < mainDialogueIDs.Count)
            {
                StartDialogue(mainDialogueIDs[mainIDIndex]);
                mainIDIndex++;

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


        private void StartDialogue(string id)
        {
            opener.StartDialogue(id);
        }

        public void Trigger()
        {
            OnTriggered?.Invoke();
            TriggerDialogue();
            triggeredAction = true; // catch for the post trigger
        }

        private void Opener_OnBoxClose()
        {
            if (triggeredAction)
            {
                OnPostTriggered?.Invoke();
                triggeredAction = false;
            }

        }

        public void DisconnectFromInteractTrigger()
        {
            OnDisconnectFromInteractTrigger?.Invoke(this);
            Component.Destroy(this);
        }


        public void EnteredTriggerZone() { }
        public void ExitedTriggerZone() { }

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

        public void AddMainDialogueElement(string dialogeID) => mainDialogueIDs.Add(dialogeID);
        public void AddBackupDialogueElement(string dialogeID) => backupDialogueIDs.Add(dialogeID);

        public void SetMainDialogue(string dialogeID)
        {
            ClearMainDialogues();
            mainDialogueIDs = new List<string>() { dialogeID};
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

    }
}

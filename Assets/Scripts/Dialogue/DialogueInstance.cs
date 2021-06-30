using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interactables;
using System;

namespace Dialogue
{
    public class DialogueInstance : MonoBehaviour, IInteractableTriggered
    {

        [SerializeField] List<string> mainDialogueIDs;
        [SerializeField] bool runMainOnlyOnce = false;
        [SerializeField] List<string> backupDialogeIDs;
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

            if(!runMainOnlyOnce && backupDialogeIDs.Count > 0)
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
            foreach (var id in backupDialogeIDs)
            {
                opener.DialogeBox.AssertContainsConversation(id);
            }
        }

        private void TriggerDialogue()
        {
            if(mainIDIndex < mainDialogueIDs.Count)
            {
                StartDialogue(mainDialogueIDs[mainIDIndex]);
                mainIDIndex++;

                if(runMainOnlyOnce && mainIDIndex >= mainDialogueIDs.Count)
                {
                    DisconnectFromInteractTrigger();
                }
                return;
            }

            if (!runMainOnlyOnce)
            {
                mainIDIndex = 0;
                TriggerDialogue();
                return;
            }

            if(backupDialogeIDs.Count > 0)
            {
                int index = backupIDIndex % backupDialogeIDs.Count;
                StartDialogue(backupDialogeIDs[index]);
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
    }
}

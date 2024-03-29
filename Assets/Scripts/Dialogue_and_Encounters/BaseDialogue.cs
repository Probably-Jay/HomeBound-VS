﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dialogue
{

    public abstract class BaseDialogue : MonoBehaviour, Interactables.IInteractableTriggered
    {
        protected DialogueBoxOpener opener;
        protected bool triggeredAction;

        public event Action OnTriggered;
        public event Action OnPostTriggered;
        public event Action<MonoBehaviour> OnDisconnectFromInteractTrigger;

        public void DisconnectFromInteractTrigger()
        {
            OnDisconnectFromInteractTrigger?.Invoke(this);
            Component.Destroy(this);
        }


        public virtual void EnteredTriggerZone() { }
        public virtual void ExitedTriggerZone() { }

        public void Trigger()
        {
            OnTriggered?.Invoke();
            TriggerDialogue();
            triggeredAction = true; // catch for the post trigger
        }

        protected virtual void Opener_OnBoxClose()
        {
            if (triggeredAction)
            {
                OnPostTriggered?.Invoke();
                triggeredAction = false;
            }
        }


        protected void StartDialogue(string id)
        {
            opener.StartDialogue(id);
        }

        protected abstract void TriggerDialogue();

        public void FindOpener()
        {
            opener = FindObjectOfType<DialogueBoxOpener>();
        }

    }
}
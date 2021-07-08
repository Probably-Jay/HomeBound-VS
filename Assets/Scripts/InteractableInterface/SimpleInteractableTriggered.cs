using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Interactables
{
    public class SimpleInteractableTriggered : MonoBehaviour, IInteractableTriggered
    {
        [SerializeField] UnityEvent interactActions;
        [SerializeField] UnityEvent enterTriggerZoneActions;
        [SerializeField] UnityEvent exitTriggerZoneActions;

        public event Action OnTriggered;
        public event Action OnPostTriggered;
        public event Action<MonoBehaviour> OnDisconnectFromInteractTrigger;


        public void Trigger()
        {
            OnTriggered?.Invoke();
            interactActions.Invoke();
            OnPostTriggered?.Invoke();
        }
        void IInteractableTriggered.EnteredTriggerZone() => enterTriggerZoneActions?.Invoke();

        void IInteractableTriggered.ExitedTriggerZone() => exitTriggerZoneActions?.Invoke();


        public void DisconnectFromInteractTrigger() { OnDisconnectFromInteractTrigger?.Invoke(this); }

    }
}
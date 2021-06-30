using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Interactables
{
    public class SimpleInteractableTrigger : MonoBehaviour, IInteractableTriggered
    {
        [SerializeField] UnityEvent actions;

        public event Action OnTriggered;
        public event Action OnPostTriggered;
        public event Action<MonoBehaviour> OnDisconnectFromInteractTrigger;


        public void Trigger()
        {
            OnTriggered?.Invoke();
            actions.Invoke();
            OnPostTriggered?.Invoke();
        }


        public void DisconnectFromInteractTrigger() { }
    }
}
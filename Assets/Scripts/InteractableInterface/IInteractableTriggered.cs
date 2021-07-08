using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Interactables
{

    public interface IInteractableTriggered
    {
        void Trigger();


        void EnteredTriggerZone();
        void ExitedTriggerZone();


        // both OnTriggered and OnPostTriggered *MUST* be called, even if the operation is a single call 

        /// <summary> Will disable the interactable until <see cref="OnPostTriggered"/> is called </summary>
        event Action OnTriggered;
        /// <summary> Will re-enable the interactable </summary>
        event Action OnPostTriggered;

        event Action<MonoBehaviour> OnDisconnectFromInteractTrigger;
        void DisconnectFromInteractTrigger();

    }
}
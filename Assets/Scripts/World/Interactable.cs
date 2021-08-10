using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System;
using Game;
using UnityEngine.EventSystems;

namespace Interactables
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class Interactable : MonoBehaviour
    {
        const float facingDirectionSensitivity = -0.9f;
        [SerializeField] string InteractionName = "Interact";
        [SerializeField] KeyCode interactKey = KeyCode.E;
        [SerializeField] bool canBeInteractedWith = true;
        [SerializeField] bool directional = true;
        private bool triggerInstantly = false;

 
        [Header("All triggers must be of type \"" + nameof(SimpleInteractableTriggered) +"\" or impliment \"" + nameof(IInteractableTriggered)+ "\"")]
        [SerializeField] List<MonoBehaviour> interactableTriggers;
  

        TMP_Text UIDisplay;


        public bool HasEvents => interactableTriggers.Count > 0;
        GameObject UIParentObject => UIDisplay.transform.parent.gameObject;

        // bool canBeInteracted = true;
        bool CanBeInteracted => canBeInteractedWith && interactableActivated && triggeredCount == 0;
        bool interactableActivated;
        private int triggeredCount = 0;
        private bool entered;

        private bool InteractUIIsEnabled => UIParentObject.activeInHierarchy; // implicit state, defined by if "interact (E)" ui is visible


        private void Awake()
        {
            UIDisplay = GetComponentInChildren<TMP_Text>();
            UIDisplay.text = $"{InteractionName} ({interactKey})";

            UIParentObject.SetActive(false);

        }

        private void OnValidate()
        {
            foreach (var item in interactableTriggers)
            {
                if (item!= null && !(item is IInteractableTriggered triggered))
                {
                    throw new Exception($"Triggers must be of type {typeof(IInteractableTriggered)}, {nameof(item)}:{item.ToString()} is type {item.GetType()}");
                }
            }
        }

        private void OnEnable()
        {
            Game.GameContextController.Instance.OnContextChange += HandleGameContextChange;

            foreach (var item in interactableTriggers)
            {
                if (item is IInteractableTriggered triggered)
                {
                    triggered.OnTriggered += Triggered_OnTriggered;
                    triggered.OnPostTriggered += Triggered_OnPostTriggered;

                    triggered.OnDisconnectFromInteractTrigger += DisconnectItem;
                }
                else
                {
                    throw new Exception($"Triggers must be of type {typeof(IInteractableTriggered)}");
                }

            }
           
        }

        private void OnDisable()
        {
            if (GameContextController.InstanceExists)
            {
                Game.GameContextController.Instance.OnContextChange -= HandleGameContextChange;
            }

            foreach (var item in interactableTriggers)
            {
                if (item is IInteractableTriggered triggered)
                {
                    triggered.OnTriggered -= Triggered_OnTriggered;
                    triggered.OnPostTriggered -= Triggered_OnPostTriggered;

                    triggered.OnDisconnectFromInteractTrigger -= DisconnectItem;
                }
            }
        }

        private void HandleGameContextChange(Context current, Context _)
        {
            switch (current)
            {
                case Context.Explore:
                    ActivateInteractable();
                    break;
                case Context.Dialogue:
                    DeactivateInteractable();
                    break;
                case Context.Rythm:
                    DeactivateInteractable();
                    break;
            }
        }

        private void ActivateInteractable() => interactableActivated = true;

        private void DeactivateInteractable()
        {
            DeactivateUI();
            interactableActivated = false;
        }



        private void Update()
        {
            if (CanBeInteracted && InteractUIIsEnabled && GetIfShouldTriggerInteraction())
            {
                HandleInteraction();
            }
        }

        private bool GetIfShouldTriggerInteraction()
        {
            return triggerInstantly || Input.GetKeyDown(interactKey);
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (!CanBeInteracted)
            {
                return;
            }

            if (!collision.CompareTag("Player"))
            {
                return;
            }

            if (!entered)
            {
                InvokeEnterInteractable();
            }

            if (directional && !PlayerFacingUs(collision.gameObject))
            {
                DeactivateUI();
                return;
            }

            ActivateUI();

        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player"))
            {
                return;
            }
            entered = true;
            InvokeEnterInteractable();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Player"))
            {
                return;
            }
            if (triggeredCount > 0)
            {
                Debug.LogWarning("Triggered count is not 0");
            }
            InvokeExitInteractable();
            entered = false;
            DeactivateUI();
        }

        private void HandleInteraction()
        {
            if (!HasEvents)
            {
                Debug.LogWarning("Cannot invoke interactable as it has no interactions associated with it");
                return;
            }

            InvokeTriggers();

            DeactivateUI();
        }

        private void InvokeTriggers()
        {
            for (int i = interactableTriggers.Count-1; i >= 0; i--) // rev in case of removal
            {
                IInteractableTriggered triggered = GetITriggered(i);
                triggered.Trigger();
            }
        }

        void InvokeEnterInteractable()
        {
            for (int i = interactableTriggers.Count - 1; i >= 0; i--)
            {
                IInteractableTriggered triggered = GetITriggered(i);
                triggered.EnteredTriggerZone();
            }
        }
        void InvokeExitInteractable()
        {
            for (int i = interactableTriggers.Count - 1; i >= 0; i--)
            {
                IInteractableTriggered triggered = GetITriggered(i);
                triggered.ExitedTriggerZone();
            }
        }

        private IInteractableTriggered GetITriggered(int i)
        {
            MonoBehaviour item = interactableTriggers[i];
            if (!(item is IInteractableTriggered triggered))
            {
                throw new Exception($"Triggers must be of type {typeof(IInteractableTriggered)}");
            }

            return triggered;
        }




        private void ActivateUI()
        {
            if (!HasEvents)
            {
                return;
            }
            UIParentObject.SetActive(true); // this sets InInteractionRange to true
        }


        private void DeactivateUI()
        {
            UIParentObject.SetActive(false); // this sets InInteractionRange to false
        }

        private bool PlayerFacingUs(GameObject playerObject)
        {
            var player = playerObject.GetComponent<Overworld.PlayerCharacterController>();

            Vector2 usToPlayer = (playerObject.transform.position - transform.position).normalized;

            Vector2 playerFacing = player.FacingVector;

            var allignment = Vector2.Dot(usToPlayer, playerFacing);

            return allignment < facingDirectionSensitivity; // if us to player is oposite direction to player is facing
        }

        public void TestFunction()
        {
            Debug.Log("Test");
        }

        private void DisconnectItem(MonoBehaviour obj)
        {
            interactableTriggers.Remove(obj);
            if (!(obj is IInteractableTriggered triggered))
            {
                throw new Exception($"Triggers must be of type {typeof(IInteractableTriggered)}");

            }
            triggered.OnTriggered -= Triggered_OnTriggered;
            triggered.OnPostTriggered -= Triggered_OnPostTriggered;

        }

        private void Triggered_OnTriggered()
        {
            triggeredCount++;
        }

        private void Triggered_OnPostTriggered()
        {
            triggeredCount--;
            if(triggeredCount < 0)
            {
                Debug.LogError("More post triggers than triggers");
                triggeredCount = 0;
            }
        }
    }
}



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
        const float facintDirectionSensitivity = -0.9f;
        [SerializeField] string InteractionName = "Interact";
        [SerializeField] KeyCode interactKey = KeyCode.E;


        // [SerializeField] UnityEvent simpleInteractions;

        // event Action interactAction; 

        [SerializeField] List<MonoBehaviour> interactableTriggers;
       
      //  readonly List<UnityEngine.Object> Othertriggers = new List<UnityEngine.Object>();


        TMP_Text UIDisplay;
      // private bool keyPressed;

        public bool HasEvents => interactableTriggers.Count > 0;
        GameObject UIParentObject => UIDisplay.transform.parent.gameObject;

        bool canBeInteracted = true;
        private bool InInteractioinRange => UIParentObject.activeInHierarchy; // implicit state, defined by if "interact (E)" ui is visible


        private void Awake()
        {
            UIDisplay = GetComponentInChildren<TMP_Text>();
            UIDisplay.text = $"{InteractionName} ({interactKey.ToString()})";

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

        private void ActivateInteractable() => canBeInteracted = true;

        private void DeactivateInteractable()
        {
            DeactivateUI();
            canBeInteracted = false;
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (!collision.CompareTag("Player"))
            {
                return;
            }
            DeactivateUI();
        }

        private void Update()
        {
            if (canBeInteracted && InInteractioinRange && Input.GetKeyDown(interactKey))
            {
                HandleInteraction();
            }
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (!canBeInteracted)
            {
                return;
            }


            if (!collision.CompareTag("Player"))
            {
                return;
            }

            if (!PlayerFacingUs(collision.gameObject))
            {
                DeactivateUI();
                return;
            }

            ActivateUI();

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
            for (int i = interactableTriggers.Count-1; i >= 0; i--)
            {
                MonoBehaviour item = interactableTriggers[i];
                if (!(item is IInteractableTriggered triggered))
                {
                    throw new Exception($"Triggers must be of type {typeof(IInteractableTriggered)}");
                }
                triggered.Trigger();
            }
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
            var player = playerObject.GetComponent<Overworld.CharacterController>();

            Vector2 usToPlayer = (playerObject.transform.position - transform.position).normalized;

            Vector2 playerFacing = player.FacingDirection;

            var allignment = Vector2.Dot(usToPlayer, playerFacing);

            return allignment < facintDirectionSensitivity; // if us to player is oposite direction to player is facing
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
            canBeInteracted = false;
        }

        private void Triggered_OnPostTriggered()
        {
            canBeInteracted = true;
        }
    }
}



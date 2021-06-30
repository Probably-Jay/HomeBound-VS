using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System;
using Game;

namespace Interactables
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class Interactable : MonoBehaviour
    {
        const float facintDirectionSensitivity = -0.9f;
        [SerializeField] string InteractionName = "Interact";
        [SerializeField] KeyCode interactKey = KeyCode.E;
        
        [SerializeField] UnityEvent interactAction;



        TMP_Text UIDisplay;
      // private bool keyPressed;

        public bool HasEvents => interactAction.GetPersistentEventCount() > 0;
        GameObject UIParentObject => UIDisplay.transform.parent.gameObject;

        bool canBeInteracted = true;
        private bool InInteractioinRange => UIParentObject.activeInHierarchy; // implicit state, defined by if "interact (E)" ui is visible

        private void Awake()
        {
            UIDisplay = GetComponentInChildren<TMP_Text>();
            UIDisplay.text = $"{InteractionName} ({interactKey.ToString()})";

            UIParentObject.SetActive(false);
        }


        private void OnEnable()
        {
            Game.GameContextController.Instance.OnContextChange += HandleGameContextChange;
        }

        private void OnDisable()
        {
            if (GameContextController.InstanceExists)
            {
                Game.GameContextController.Instance.OnContextChange -= HandleGameContextChange;
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


            interactAction.Invoke();
            DeactivateUI();
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

    }
}



using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System;

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

        public bool HasEvents => interactAction.GetPersistentEventCount() > 0;
        GameObject UIParentObject => UIDisplay.transform.parent.gameObject;
        

        private void Awake()
        {
            UIDisplay = GetComponentInChildren<TMP_Text>();
            UIDisplay.text = $"{InteractionName} ({interactKey.ToString()})";

            UIParentObject.SetActive(false);
        }

        /*
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag("Player"))
            {
                return;
            }

            if (!PlayerFacingUs(collision.gameObject))
            {
                return;
            }

            ActivateUI();
        }



    */
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (!collision.CompareTag("Player"))
            {
                return;
            }
            DeactivateUI();
        }


        private void OnTriggerStay2D(Collider2D collision)
        {
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

            if (Input.GetKeyDown(interactKey))
            {
                HandleInteraction();
            }

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
            UIParentObject.SetActive(true);
        }


        private void DeactivateUI()
        {
            UIParentObject.SetActive(false);
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



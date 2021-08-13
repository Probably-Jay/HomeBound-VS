using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interactables;
using Dialogue;

namespace QuestLogic
{
    public class PortMan : MonoBehaviour
    {

        [SerializeField] string dialogue;
        Animator animator;
        Interactable interactable;
        DialogueInstance dialogueInstance;
        
        private void Awake()
        {
            animator = GetComponent<Animator>();
            this.AssignComponent(out animator);

            this.AssignComponent(out interactable);
            this.AssignComponent(out dialogueInstance);

            this.NullCheck(dialogue, nameof(dialogue));
        }

        public void AddDialogue()
        {
            dialogueInstance.AddMainDialogueElement(dialogue);
            interactable.InteractionsEnabled = true;
        }

        public void WakeUp()
        {
            animator.SetBool("Awake", true);
        }

    }
}

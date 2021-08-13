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
            this.NullCheck(animator);
        }

        public void WakeUp()
        {
            animator.SetBool("Awake", true);

        }

    }
}

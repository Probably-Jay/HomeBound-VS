using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Quests
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class ColliderQuestStep : SimpleQuestStep
    {
        [SerializeField] bool UncompleteOnExit = false;


        private void OnTriggerEnter2D(Collider2D _)
        {
            CompleteStep();
        }

        private void OnTriggerEnter(Collider _)
        {
            if (UncompleteOnExit)
            {
                UnCompleteStep();
            }
        }


        // make sure collider is trigger
        private void Start()
        {
            SetTrigger();
        }
#if UNITY_EDITOR
        private void OnValidate()
        {
            SetTrigger();
        }
#endif
        private void SetTrigger()
        {
            GetComponent<BoxCollider2D>().isTrigger = true;
        }
    }
}
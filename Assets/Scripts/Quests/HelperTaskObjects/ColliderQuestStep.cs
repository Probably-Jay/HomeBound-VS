using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Quests
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class ColliderQuestStep : SimpleQuestStep
    {
        [SerializeField] bool UnCompleteOnExit = false;

        private void Start()
        {
            SetTrigger();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            CompleteStep();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (UnCompleteOnExit)
            {
                UnCompleteStep();
            }
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
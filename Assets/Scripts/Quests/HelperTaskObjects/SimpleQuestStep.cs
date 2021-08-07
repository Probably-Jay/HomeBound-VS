using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Quests
{
    [System.Serializable]
    public class SimpleQuestStep : QuestStepCompletable
    {
        [Tooltip("Keep this false if the task should not start completed")]
        [SerializeField] private bool completed;

        public override bool Completed
        {
            get => completed;
            protected set => completed = value;
        }

        public override event Action OnCompleted;

        public void CompleteStep()
        {
            Completed = true;
            OnCompleted?.Invoke();
        }

        public void UnCompleteStep() => Completed = false;

#if UNITY_EDITOR
        bool completedLastCheck;
        private void OnValidate()
        {
            if (completedLastCheck == completed) return;

            if (completed)
            {
                CompleteStep();
            }
            else
            {
                UnCompleteStep();
            }

            completedLastCheck = completed;
        }
#endif

    }
}

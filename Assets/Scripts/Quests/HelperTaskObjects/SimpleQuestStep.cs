using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Quests
{
    public class SimpleQuestStep : QuestTaskCompletable
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



    }
}

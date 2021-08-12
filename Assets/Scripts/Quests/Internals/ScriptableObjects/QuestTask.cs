using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Quests
{
    public class QuestTask : MonoBehaviour, IUIDesrcibable
    {
        [SerializeField] string title;
        [TextArea(1, 4)]
        [SerializeField] string description;
        [SerializeField] public List<SimpleQuestStep> taskPrerequisites;
       
        public bool TaskActive { get; set; }
        public string Title => title;

        public string Description => description;
        

        public bool Complete => taskPrerequisites.TrueForAll((t) => t.Completed);
        public bool AnyTasksCompleted => taskPrerequisites.Any((t) => t.Completed);

        public UnityEvent OnCompleteTask { get => onCompleteTask; set => onCompleteTask = value; }
        public UnityEvent OnBeginTask { get => onBeginTask; set => onBeginTask = value; }


        [SerializeField] private UnityEvent onBeginTask;
        [SerializeField] private UnityEvent onCompleteTask;

        public void Init()
        {
            if(taskPrerequisites.Count == 0)
            {
                Debug.LogError($"Quest task has no prerequisites",this);
            }
            UpdateDelegateSubscriptions();
        }

        public void CheckAndTriggerTaskComplete()
        {
            if (!TaskActive) 
            {
                return;
            }

            if (AnyTasksCompleted)
            {
                Debug.Log($"Quest step completed");
            }

            if (!Complete)
            {
                return;
            }

            OnCompleteTask?.Invoke();
        }

        private void UpdateDelegateSubscriptions()
        {
            foreach (var preRequisite in taskPrerequisites)
            {
                if(preRequisite == null)
                {
                    throw new Exception($"Task {name} pre-requisite is null");
                }
                preRequisite.OnCompleted += CheckAndTriggerTaskComplete;
            }
        }

        internal void BeginTask()
        {
            TaskActive = true;
            OnBeginTask?.Invoke();
            CheckAndTriggerTaskComplete();
        }

        internal void EndTask()
        {
            TaskActive = false;
        }
    }

    //[System.Serializable]
    //public class QuestStep 
    //{ 
    //    public int stepOrder;
    //    public QuestStepCompletable step;
    //}

}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Quests
{
   // [CreateAssetMenu(fileName = "QuestTask", menuName = "ScriptableObjects/Quests/Task", order = 1)]
    public class QuestTask : MonoBehaviour
    {
 
        [SerializeField]  private List<QuestTaskCompletable> taskPrerequisites;
       
        public bool TaskActive { get; set; }
        

        public bool TaskComplete => taskPrerequisites.TrueForAll((t) => t.Completed);
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
                Debug.Log("Quest step completed");
            }

            if (!TaskComplete)
            {
                return;
            }

            OnCompleteTask?.Invoke();
        }

        private void UpdateDelegateSubscriptions()
        {
            foreach (var preRequisite in taskPrerequisites)
            {
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
}
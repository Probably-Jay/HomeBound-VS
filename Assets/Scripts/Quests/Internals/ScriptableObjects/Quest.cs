using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Quests
{
    ///[CreateAssetMenu(fileName = "Quest", menuName = "ScriptableObjects/Quests/Quest", order = 1)]
    public class Quest : MonoBehaviour
    {
        [SerializeField] string questName;
        [SerializeField] List<QuestTask> tasks = new List<QuestTask>();
        [SerializeField] UnityEvent onQuestBegin;
        [SerializeField] UnityEvent onQuestComplete;

        public bool Complete => CurrentQuestStep >= tasks.Count;

        public bool Begun => CurrentQuestStep > -1;
        
        


        public int CurrentQuestStep { get; private set; } = -1;
        public QuestTask CurrentQuestTask
        {
            get
            {
                if (!Begun || Complete)
                {
                    Debug.LogError($"Quest {QuestName} not begun or is completed, {nameof(CurrentQuestTask)} not available");
                    return null;
                }
                return tasks[CurrentQuestStep];
            }
        }

        public string QuestName => questName;

        public UnityEvent OnQuestComplete { get => onQuestComplete; set => onQuestComplete = value; }

        public event Action OnTaskComplete;


        public void Init()
        {
            foreach (var task in tasks)
            {
                task.Init();
                task.OnCompleteTask.AddListener(Progress);
            }
        }
        public void Begin()
        {
            if (Begun)
            {
                Debug.LogError($"Quest {QuestName} already begun");
            }
            Debug.Log($"Quest {QuestName} has been begun");
            CurrentQuestStep = 0;
            CurrentQuestTask.TaskActive = true;
            onQuestBegin?.Invoke();
        }

        public void Progress()
        {
            if (Complete) return;
            if (!CurrentQuestTask.TaskComplete) return;
            OnTaskComplete?.Invoke();

            ProgressQuestTask();

        }

        private void ProgressQuestTask()
        {
            DisposeCurrentTask();
            CurrentQuestStep++;
            if (Complete)
            {
                CompleteQuest();
                return;
            }
            CurrentQuestTask.TaskActive = true;
            Debug.Log("Quest step progressed");
        }

        private void DisposeCurrentTask()
        {
            CurrentQuestTask.OnCompleteTask?.RemoveListener(Progress);
            CurrentQuestTask.TaskActive = false;
        }

        private void CompleteQuest()
        {
            OnQuestComplete?.Invoke();
            Debug.Log("Quest completed");
        }
    }
}
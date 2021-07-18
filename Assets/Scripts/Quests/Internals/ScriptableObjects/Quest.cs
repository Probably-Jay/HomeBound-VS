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

        private void Awake()
        {
            tasks.Clear();

            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                var task = child.GetComponent<QuestTask>();
                if (task != null)
                {
                    tasks.Add(task);
                }
            }

            

            // tasks = new List<QuestTask>( GetComponentsInChildren<QuestTask>());
        }

        private void Start()
        {
            Init();
        }

        private void Init()
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
            BeginCurrentTask();
            onQuestBegin?.Invoke();
        }

        private void BeginCurrentTask()
        {
            CurrentQuestTask.BeginTask();
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
            BeginCurrentTask();
            Debug.Log("Quest step progressed");
        }

        private void DisposeCurrentTask()
        {
            CurrentQuestTask.OnCompleteTask?.RemoveListener(Progress);
            CurrentQuestTask.EndTask();
        }

        private void CompleteQuest()
        {
            OnQuestComplete?.Invoke();
            Debug.Log("Quest completed");
        }
    }
}
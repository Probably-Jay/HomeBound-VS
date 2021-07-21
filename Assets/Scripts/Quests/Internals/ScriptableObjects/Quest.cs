using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Quests
{
    ///[CreateAssetMenu(fileName = "Quest", menuName = "ScriptableObjects/Quests/Quest", order = 1)]
    public class Quest : MonoBehaviour, IUIDesrcibable
    {
        [SerializeField] string title;
        [TextArea(1,4)]
        [SerializeField] string description;
        [SerializeField,HideInInspector] List<QuestTask> tasks = new List<QuestTask>();
        [SerializeField] UnityEvent onQuestBegin;
        [SerializeField] UnityEvent onQuestComplete;


        public string Title => title;
        public string Description => description;

        public List<QuestTask> Tasks { get => tasks; }
        public int CurrentTask { get; private set; } = -1;

        public bool Begun => CurrentTask > -1;
        public bool Complete => CurrentTask >= Tasks.Count;

        

        public UnityEvent OnQuestComplete { get => onQuestComplete; set => onQuestComplete = value; }

        public event Action OnTaskComplete;
        



        public void Validate()
        {
            return; // called to assure object exists
        }

        public QuestTask CurrentQuestTask
        {
            get
            {
                if (!Begun || Complete)
                {
                    Debug.LogError($"Quest {Title} not begun or is completed, {nameof(CurrentQuestTask)} not available");
                    return null;
                }
                return Tasks[CurrentTask];
            }
        }



        private void Awake()
        {
            Tasks.Clear();

            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                var task = child.GetComponent<QuestTask>();
                if (task != null)
                {
                    Tasks.Add(task);
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

            foreach (var task in Tasks)
            {
                task.Init();
                task.OnCompleteTask.AddListener(Progress);
            }
        }
        public void Begin()
        {
            if (Begun)
            {
                Debug.LogError($"Quest {Title} already begun");
            }
            Debug.Log($"Quest {Title} has been begun");
            CurrentTask = 0;
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
            CurrentTask++;
            Debug.Log("Quest step progressed");
            if (Complete)
            {
                CompleteQuest();
                return;
            }
            BeginCurrentTask();
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
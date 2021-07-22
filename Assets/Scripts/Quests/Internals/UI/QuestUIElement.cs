using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Quests
{
    /// <summary>
    /// Its a bit shit but it works
    /// </summary>
    public class QuestUIElement : MonoBehaviour, IUISelectable
    {
        private Quest quest;
        [SerializeField] Transform childParent;
        [SerializeField] GameObject taskPrefab;
        [SerializeField] GameObject DropdownButton;
        [SerializeField] private TMP_Text title;
        public List<TaskUIElement> taskElements { get; private set; } = new List<TaskUIElement>();

        Transform transformParent;
        bool collapsed;

        public event Action<IUISelectable> OnSelected;
        public event Action<QuestUIElement> OnDroppedDown;

        public IUIDesrcibable Describable => quest;
        public Quest Quest => quest;

        private void Awake()
        {
            transformParent = transform.parent;
        }

        public void Init(Quest quest)
        {
            this.quest = quest;
            title.text = quest.Title;
            InitTasks();

            Deselect();
            Collapse();
        }

        private void InitTasks()
        {
            foreach (var task in quest.Tasks)
            {
                var taskGo = Instantiate(taskPrefab, transform);
                var taskUI = taskGo.GetComponent<TaskUIElement>();
                taskUI.Init(task);
                taskElements.Add(taskUI);
            }
        }


        public void CollapseOrExpand()
        {
            if (collapsed)
            {
                Expand();
            }
            else
            {
                Collapse();
            }
        }

        private void Expand()
        {
            int thisIndex = transform.GetSiblingIndex();
            bool lastTask = false;
            for (int i = 0; i < taskElements.Count && !lastTask; i++) // this is where the tasks that will be displayed are updated, really that should not be calculated here
            {
                TaskUIElement task = taskElements[i];
                if (!task.task.Complete)
                {
                    lastTask = true;
                }
                Transform child = task.transform;
                child.gameObject.SetActive(true);
                child.SetParent(transformParent);
                child.SetSiblingIndex(thisIndex + 1 + i);
            }
            collapsed = false;
            DropdownButton.transform.rotation = Quaternion.identity;
            OnDroppedDown?.Invoke(this);
        }

        public void Collapse()
        {
            foreach (var child in taskElements)
            {
                child.transform.SetParent(childParent);
                child.gameObject.SetActive(false);
            }
            collapsed = true;
            DropdownButton.transform.rotation = Quaternion.Euler(0, 0, 90);
        }

        public void Select()
        {
            OnSelected?.Invoke(this);
            GetComponent<UnityEngine.UI.Image>().color = new Color(0.5f, 0.5f, 1f);
        }

        public void Deselect()
        {
            GetComponent<UnityEngine.UI.Image>().color = new Color(1f, 1f, 1f);

        }

        internal void UpdateQuest()
        {
            if (quest.Complete)
            {
                title.text = $"<s>{title.text}</s>";
                GetComponent<UnityEngine.UI.Image>().color = new Color(0.35f, 0.35f, 0.35f);
                transform.SetAsLastSibling();
                Collapse();
            }
            foreach (var task in taskElements)
            {
                task.UpdateTask();
            }

            if (!collapsed)
            {
                Collapse();
                Expand();
            }

        }
    }
}
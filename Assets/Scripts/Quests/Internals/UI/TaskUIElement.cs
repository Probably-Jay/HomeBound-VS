using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Quests
{
    public class TaskUIElement : MonoBehaviour, IUISelectable
    {
        [SerializeField]private TMP_Text title;
        public QuestTask task { get; private set; }

        public event Action<IUISelectable> OnSelected;

        public IUIDesrcibable Describable => task;



        public void Init(QuestTask task)
        {
            this.task = task;
            title.text = task.Title;
            Deselect();
        }

        public void Select()
        {
            OnSelected?.Invoke(this);
            GetComponent<UnityEngine.UI.Image>().color = new Color(0.4f, 0.4f, 0.95f);
        }

        public void Deselect()
        {
            GetComponent<UnityEngine.UI.Image>().color = new Color(0.8f, 0.8f, 0.8f);
        }

        internal void UpdateTask()
        {
            if (task.Complete)
            {
                title.text = $"<s>{title.text}</s>";
                GetComponent<UnityEngine.UI.Image>().color = new Color(0.3f, 0.3f, 0.3f);
                transform.SetAsLastSibling();
            }

        }
    }
}
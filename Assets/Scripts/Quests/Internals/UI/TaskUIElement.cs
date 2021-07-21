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
        private QuestTask task;

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
            GetComponent<UnityEngine.UI.Image>().color = new Color(0.5f, 0.5f, 1f);
        }

        public void Deselect()
        {
            GetComponent<UnityEngine.UI.Image>().color = new Color(0.5f, 0.5f, 0.5f);
        }

    }
}
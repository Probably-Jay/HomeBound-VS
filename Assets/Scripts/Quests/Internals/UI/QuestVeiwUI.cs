using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

namespace Quests
{
    public class QuestVeiwUI : MonoBehaviour
    {
        [SerializeField] QuestJournal playerQuestJournal;

        [Header("Internals")]
        [SerializeField] GameObject QuestElementPrefab;

        [SerializeField] Transform veiwContentParent;

        [SerializeField] GameObject infoPannel;
        [SerializeField] TMP_Text title;
        [SerializeField] TMP_Text body;

        private List<QuestUIElement> questElements = new List<QuestUIElement>();
        private List<IUISelectable> selectables = new List<IUISelectable>();
        private bool inited;

        private void OnEnable()
        {
            UnsetSelected();
            if (!inited)
                return;
            Subscribe();
            UpdateQuests();
            CheckForNewQuests();
        }

   

        private void OnDisable()
        {
            UnsetSelected();
          //  playerQuestJournal.OnQuestChange -= PlayerQuestJournal_OnQuestChange;
            if (!inited)
                return;
            UnSubscribe();
        }


        private void Awake()
        {
            playerQuestJournal.OnQuestChange += PlayerQuestJournal_OnQuestChange;
        }

        private void OnDestroy()
        {
            playerQuestJournal.OnQuestChange -= PlayerQuestJournal_OnQuestChange;
        }

        // Start is called before the first frame update
        void Start()
        {
            AddQuests();
            Subscribe();
            inited = true;
        }

        private void Subscribe()
        {
            foreach (var element in questElements)
            {
                element.OnDroppedDown += Element_OnDroppedDown;
            }
            foreach (var selectable in selectables)
            {
                selectable.OnSelected += Selectable_OnSelected;
            }
        }


        private void UnSubscribe()
        {
            foreach (var element in questElements)
            {
                element.OnDroppedDown -= Element_OnDroppedDown;
            }
            foreach (var selectable in selectables)
            {
                selectable.OnSelected -= Selectable_OnSelected;
            }
        }

        private void PlayerQuestJournal_OnQuestChange()
        {
            AddQuests();
            if (!inited)
                return;
            UnSubscribe();
            Subscribe();
        }

        private void Element_OnDroppedDown(QuestUIElement expanded)
        {
            foreach (var element in questElements)
            {
                if (element == expanded)
                {
                    continue;
                }
                else
                {
                    element.Collapse();
                }
            }
        }

        private void Selectable_OnSelected(IUISelectable selected)
        {
            foreach (var element in selectables)
            {
                if (element == selected)
                {
                    continue;
                }

                element.Deselect();
            }

            SetSelected(selected);
        }

        private void SetSelected(IUISelectable selected)
        {
            infoPannel.SetActive(true);
            title.text = selected.Describable.Title;
            body.text = selected.Describable.Description;
        }
        private void UnsetSelected()
        {
            infoPannel.SetActive(false);
            foreach (var element in selectables)
            {
                element.Deselect();
            }
        }
        private void CheckForNewQuests()
        {

            var distinct = (from q in questElements
                            where !playerQuestJournal.Quests.Contains(q.Quest)
                            select q).ToList();

            if (distinct.Count > 0)
            {
                AddQuests();
            }
        }

        private void AddQuests()
        {
            ClearAll();
            foreach (var quest in playerQuestJournal.Quests)
            {
                AddQuest(quest);
            }
            UpdateQuests();
        }

        private void AddQuest(Quest quest)
        {
            var elementGo = Instantiate(QuestElementPrefab, veiwContentParent);
            var questElement = elementGo.GetComponent<QuestUIElement>();
            questElement.Init(quest);
            questElements.Add(questElement);
            selectables.Add(questElement);
            foreach (var taskElement in questElement.taskElements)
            {
                selectables.Add(taskElement);
            }
        }

        private void ClearAll()
        {
            for (int i = veiwContentParent.childCount - 1; i >= 0; i--)
            {
                Destroy(veiwContentParent.GetChild(i).gameObject);
            }
        }

        private void UpdateQuests()
        {
            foreach (var quest in questElements)
            {
                quest.UpdateQuest();
            }
        }
    }
}

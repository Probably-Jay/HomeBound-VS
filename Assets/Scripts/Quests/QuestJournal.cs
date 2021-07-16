using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quests
{
    public class QuestJournal : MonoBehaviour
    {
        [SerializeField] List<Quest> currentQuests;


        public List<Quest> CurrentQuests { get => currentQuests; set => currentQuests = value; }
        
        void Start()
        {
            InitInitialQuests();
        }

        private void InitInitialQuests()
        {
            foreach (var quest in currentQuests)
            {
                quest.Init();
                BeginQuest(quest);
            }
        }

        public void AddAndBeginQuest(Quest quest)
        {
            CurrentQuests.Add(quest);
            BeginQuest(quest);
        }

        private void BeginQuest(Quest quest)
        {
            quest.Begin();
            quest.OnQuestComplete.AddListener(RemoveQuest);
        }

        private void RemoveQuest()
        {
            var toRemove = new List<Quest>();
            foreach (var quest in CurrentQuests)
            {
                if (quest.Complete)
                {
                    quest.OnQuestComplete.RemoveListener(RemoveQuest);
                    toRemove.Add(quest);
                }
            }

            foreach (var quest in toRemove)
            {
                currentQuests.Remove(quest);
            }
        }
    }
}
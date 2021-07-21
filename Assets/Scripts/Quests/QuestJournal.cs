using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quests
{
    public class QuestJournal : MonoBehaviour, IQuestHolder
    {
        [SerializeField] List<Quest> currentQuests = new List<Quest>();


        public List<Quest> Quests { get => currentQuests; set => currentQuests = value; }


        private void Start()
        {
            foreach (var quest in Quests)
            {
                try
                {
                    quest.Validate();
                }
                catch (System.Exception)
                {
                    Debug.LogError($"Quest {quest} is null or invalid.");
                    throw;
                }
            }
        }

        public void ReceiveQuest(Quest quest)
        {
            Quests.Add(quest);
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
            foreach (var quest in Quests)
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
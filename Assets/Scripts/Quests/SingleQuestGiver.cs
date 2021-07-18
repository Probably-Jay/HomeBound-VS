using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Quests
{
    public class SingleQuestGiver : MonoBehaviour, IQuestHolder
    {
        [SerializeField] Quest quest;

        //public Quest Quest { get => quest; }



        public void RemoveQuest()
        {
            quest = null;
        }

        public void ReceiveQuest(Quest quest)
        {
            this.quest = quest;
        }

        public void PassQuest(IQuestHolder receiver)
        {
            receiver.ReceiveQuest(quest);
            RemoveQuest();
        }


        
    }
}
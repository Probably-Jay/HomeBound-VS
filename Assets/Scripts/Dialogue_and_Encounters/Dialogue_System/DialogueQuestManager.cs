using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dialogue
{
    public class DialogueQuestManager : MonoBehaviour
    {
        [SerializeField, HideInInspector] public List<Internal.StringQuestDict> displayQuests;
        [SerializeField] Quests.QuestJournal playerQuestJournal;
        Dictionary<string, Quests.Quest> quests = new Dictionary<string, Quests.Quest>();
        Dictionary<string, Quests.Quest> givenQuests = new Dictionary<string, Quests.Quest>();

        private void Awake()
        {
            foreach (var quest in displayQuests)
            {     
                quests.Add(quest.id, quest.quest);
            }
        }

        private void Start()
        {
            foreach (var quest in displayQuests)
            {
                try
                {
                    quest.quest.Validate();
                }
                catch (System.Exception)
                {
                    Debug.LogError($"QuestID {quest.id}'s quest {quest.quest} is null or invalid.");
                    throw;
                }
            }
        }
        public bool HasTask(string ID) => quests.ContainsKey(ID);

        public void PassQuest(string ID)
        {
            AssertContainsTaskID(ID);
            var quest = quests[ID];
            playerQuestJournal.ReceiveQuest(quest);

            quests.Remove(ID);
            givenQuests.Add(ID, quest);
        }

       
        private void AssertContainsTaskID(string ID)
        {
            if (!HasTask(ID))
            {
                if (!givenQuests.ContainsKey(ID))
                    throw new System.Exception($"The quest task id {ID} does not exist in {nameof(DialogueQuestTaskManager)}");
                else
                    Debug.LogWarning($"The quest task id {ID} has already been given");
            }
        }

    }

    namespace Internal
    {
        [System.Serializable]
        public struct StringQuestDict
        {
            public string id;
            public Quests.Quest quest;
        }

    }

}



using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quests
{
    public class QuestVeiwUI : MonoBehaviour
    {
        [SerializeField] GameObject QuestElementPrefab;

        [SerializeField] Transform veiwContentParent;
        [SerializeField] QuestJournal playerQuestJournal;
        

        // Start is called before the first frame update
        void Start()
        {

                AddQuests();
            
        }

        private void AddQuests()
        {
            ClearAll();
            for (int i = 0; i < 5; i++)
            {
                foreach (var quest in playerQuestJournal.Quests)
                {
                    AddQuest(quest);
                }
            }
        }

        private void AddQuest(Quest quest)
        {
            var elementGo = Instantiate(QuestElementPrefab,veiwContentParent);
            var element = elementGo.GetComponent<QuestUIElement>();
            element.Init(quest);
        }

        private void ClearAll()
        {
            for (int i = veiwContentParent.childCount - 1; i >= 0; i--)
            {
                Destroy(veiwContentParent.GetChild(i).gameObject);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

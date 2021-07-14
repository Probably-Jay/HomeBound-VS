using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Quests
{
    public class QuestHolder : MonoBehaviour
    {
        [SerializeField] List<Quest> quests = new List<Quest>();

      

        private void Start()
        {
            InitQuests();
        }

        private void InitQuests()
        {
            foreach (var quest in quests)
            {
                quest.Init();
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Dialogue
{
    public class DialogueQuestTasks : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

    namespace Internal
    {
        [System.Serializable]
        public struct StringQuestDict 
        { 
            public string id;
            public Quests.SimpleQuestStep quest;
        }

    }

}
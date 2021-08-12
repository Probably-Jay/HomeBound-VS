using Overworld;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Quests
{
    [System.Serializable]
  
    public class RoomQuestStep : SimpleQuestStep
    {
        [SerializeField] bool CompleteOnRoomExit = false;
        [Header("Doors")]
        [SerializeField] Door entrance;
        [SerializeField] Door exit;

        private void OnEnable()
        {
            if (!CompleteOnRoomExit)
            {
                entrance.OnEnterDoor += CompleteStep;
                exit.OnEnterDoor += UnCompleteStep;
            }
            else
            {
                entrance.OnEnterDoor += UnCompleteStep;
                exit.OnEnterDoor += CompleteStep;
            }
        }
    }
}
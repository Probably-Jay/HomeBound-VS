using System;
using UnityEngine;

namespace Quests
{
  // [System.Serializable]
    public abstract class QuestStepCompletable : MonoBehaviour
    {
        public virtual bool Completed { get; protected set; }
        public virtual event Action OnCompleted;
    }
}
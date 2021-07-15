using System;
using UnityEngine;

namespace Quests
{
    public abstract class QuestTaskCompletable : MonoBehaviour
    {
        public virtual bool Completed { get; protected set; }
        public virtual event Action OnCompleted;
    }
}
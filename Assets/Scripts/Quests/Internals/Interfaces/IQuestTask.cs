using UnityEngine.Events;

namespace Quests
{
    public interface IQuestTask
    {
        bool TaskComplete { get; }
        bool TaskActive { get; set; }

        void Init();
        UnityEvent OnCompleteTask { get; }
        UnityEvent OnBeginTask { get; }

    }
}
using System;

namespace Quests
{
    public interface IUISelectable
    {
        void Select();
        void Deselect();
        IUIDesrcibable Describable {get;}

        event Action<IUISelectable> OnSelected;
    }
}
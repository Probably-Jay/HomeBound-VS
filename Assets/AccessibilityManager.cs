using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SingletonManagement;

namespace Accessibility
{
    public class AccessibilityManager : Singleton<AccessibilityManager>
    {
        public override void Initialise()
        {
            base.InitSingleton();
        }

       // public ColourBlindHelper ColourBlindHelper { get; } = new ColourBlindHelper();
    }
}

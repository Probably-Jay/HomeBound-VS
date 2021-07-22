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
            if (PlayerPrefs.HasKey(ColourBlindPrefs.ColourModePrefsKey))
            {
                ColourBlindHelper.Mode = (ColourBlindMode)PlayerPrefs.GetInt(ColourBlindPrefs.ColourModePrefsKey);
            }
            else
            {
                ColourBlindHelper.Mode = ColourBlindMode.Trichromacy;
            }
        }

        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Accessibility
{
    public class ColourBlindPrefs : MonoBehaviour
    {
        public const string ColourModePrefsKey = "_colourModePrefs";
        [SerializeField]TMP_Dropdown dropdown;

        public void SaveChangedValue(int value)
        {
            PlayerPrefs.SetInt(ColourModePrefsKey, value);
        }

    }
}
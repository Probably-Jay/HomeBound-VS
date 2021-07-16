using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePrefs : MonoBehaviour
{
    public void SaveAllPrefs()
    {
        Debug.Log($"Shake set at {PlayerPrefs.GetFloat(Accessibility.ScreenShakePrefs.ScreenShakePrefsKey)}");
        PlayerPrefs.Save();
    }
}

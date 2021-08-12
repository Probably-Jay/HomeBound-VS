using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempDisabler : MonoBehaviour
{
    private void Awake()
    {
        if(Application.platform != RuntimePlatform.WindowsEditor)
        {
            Destroy(gameObject);
        }
    }
}

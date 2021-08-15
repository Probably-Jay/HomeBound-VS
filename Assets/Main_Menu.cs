using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main_Menu : MonoBehaviour
{
    //EM - For start button
    public void PlayGame()
    {
        SceneChange.SceneChangeController.Instance.ChangeScene(SceneChange.Scenes.Game);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main_Menu : MonoBehaviour
{
    //Script by Ella-May
    //Start Button
    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    
    //Quit Button
    public void QuitGame()
    {
        Debug.Log ("QUIT!");
        Application.Quit();
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class MainMenuUI_Manager : MonoBehaviour
{
    public void Button_Start()
    {
        SceneManager.LoadScene("GameScene");
    }
    
    public void Button_Quit()
    {
        #if UNITY_EDITOR
        EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }
}

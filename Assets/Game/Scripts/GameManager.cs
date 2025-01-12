using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Character _playerCharacter;
    private bool GameIsOver;
    public GameUI_Manager UI_Manager;
    
    private void Awake()
    {
        _playerCharacter = GameObject.FindWithTag("Player").GetComponent<Character>();
    }

    private void GameOver()
    {
        UI_Manager.ShowGameOverUI();
    }
    
    public void GameIsFinished()
    {
        UI_Manager.ShowGameIsFinishedUI();
    }
    
    // Update is called once per frame
    void Update()
    {
        if(GameIsOver) {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UI_Manager.TogglePauseUI();
        }

        if (_playerCharacter.CurrentState == Character.CharacterState.Dead)
        {
            GameIsOver = true;
            GameOver();
        }
    }
    
    public void ReturnToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

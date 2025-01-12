using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameUI_Manager : MonoBehaviour
{
    public GameManager GM;

    public TMPro.TextMeshProUGUI CoinText;
    public Slider HeathSlider;
    public GameObject UI_Pause;
    public GameObject UI_GameOver;
    public GameObject UI_GameIsFinished;
    
    private enum UIState
    {
        Gameplay,
        Pause,
        GameOver,
        GameIsFinished
    }
    
    UIState currentState;

    private void Start()
    {
        SwitchUIState(UIState.Gameplay);
    }
    
    private void Update()
    {
        HeathSlider.value = GM._playerCharacter.GetComponent<Health>().currentHealthPercent;
        CoinText.text = GM._playerCharacter.GetComponent<Character>().coin.ToString();
    }

    private void SwitchUIState(UIState state)
    {
        UI_Pause.SetActive(false);
        UI_GameOver.SetActive(false);
        UI_GameIsFinished.SetActive(false);

        Time.timeScale = 1;
        
        switch (state)
        {
            case UIState.Gameplay:
                break;
            case UIState.Pause:
                UI_Pause.SetActive(true);
                Time.timeScale = 0;
                break;
            case UIState.GameOver:
                UI_GameOver.SetActive(true);
                break;
            case UIState.GameIsFinished:
                UI_GameIsFinished.SetActive(true);
                break;
        }
        
        currentState = state;
    }

    public void TogglePauseUI()
    {
        if (currentState == UIState.Gameplay)
        {
            SwitchUIState(UIState.Pause);
        } else if (currentState == UIState.Pause)
        {
            SwitchUIState(UIState.Gameplay);
        }
    }

    public void Button_MainMenu()
    {
        GM.ReturnToMainMenu();
    }
    
    public void Button_Restart()
    {
        GM.Restart();
    }

    public void ShowGameOverUI()
    {
        SwitchUIState(UIState.GameOver);
    }
    
    public void ShowGameIsFinishedUI()
    {
        SwitchUIState(UIState.GameIsFinished);
    }
}

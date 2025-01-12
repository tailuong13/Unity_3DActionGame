using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private Character _playerCharacter;
    private bool GameIsOver;
    
    private void Awake()
    {
        _playerCharacter = GameObject.FindWithTag("Player").GetComponent<Character>();
    }

    private void GameOver()
    {
        Debug.Log("Game Over");
    }
    
    public void GameIsFinished()
    {
        Debug.Log("Game is finished");
    }
    
    // Update is called once per frame
    void Update()
    {
        if(GameIsOver) {
            return;
        }

        if (_playerCharacter.CurrentState == Character.CharacterState.Dead)
        {
            GameIsOver = true;
            GameOver();
        }
    }
}

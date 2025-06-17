using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
/**/
[DisallowMultipleComponent]
public class SettingsManager : SingletonMonobehaviour<SettingsManager>
{
    #region Header GAMEOBJECT REFERENCES
    [Space(10)]
    [Header("ССЫЛКИ НА GAMEOBJECT")]
    #endregion Header GAMEOBJECT REFERENCES


    #region Tooltip

    [Tooltip("Заполнить игровым объектом меню паузы в иерархии")]

    #endregion Tooltip

    [SerializeField] private GameObject pauseMenu;


    [HideInInspector] public GameState gameState;
    [HideInInspector] public GameState previousGameState;


    private void Start()
    {
        previousGameState = GameState.gameStarted;
        gameState = GameState.gameStarted;
    }
    private void HandleGameState()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGameMenu();
        }
    }

    private void Update()
    {
        HandleGameState();
    }


    public void PauseGameMenu()
    {
        if (gameState != GameState.gamePaused)
        {
            pauseMenu.SetActive(true);

            previousGameState = gameState;
            gameState = GameState.gamePaused;
        }
        else if (gameState == GameState.gamePaused)
        {
            pauseMenu.SetActive(false);
    
            gameState = previousGameState;
            previousGameState = GameState.gamePaused;

        }
    }

}
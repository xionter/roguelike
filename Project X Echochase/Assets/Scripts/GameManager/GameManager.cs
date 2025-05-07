using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class GameManager : SingletonMonobehaviour<GameManager>
{
    #region Header Dungeon Levels

    [Space(10)]
    [Header("Dungeon Levels")]
    
    #endregion
    
    #region ToolTip
    
    [Tooltip("Заполнить данжн левелами")]
    
    #endregion
    
    [SerializeField] private List<DungeonLevelSO> dungeonLevelList;

    #region ToolTip

    [Tooltip("Заполнить начальными данжн левелами, первый левел = 0")]

    #endregion
    
    [SerializeField] private int currentDungeonLevelIndex = 0;
    [HideInInspector] public GameState gameState;
    
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        gameState = GameState.gameStarted;
    }

    // Update is called once per frame
    private void Update()
    {
        HandleGameState();
        if (Input.GetKeyDown(KeyCode.R))
            gameState = GameState.gameStarted;
    }

    private void HandleGameState()
    {
        switch (gameState)
        {
            case GameState.gameStarted:
                PlayDungeonLevel(currentDungeonLevelIndex);
                
                gameState = GameState.playingLevel;

                break;
            
        }
    }

    private void PlayDungeonLevel(int currentDungeonLevelIndex)
    {
        
        var dungeonBuiltSucessfully = DungeonBuilder.Instance.GenerateDungeon(dungeonLevelList[currentDungeonLevelIndex]);
        
        if (!dungeonBuiltSucessfully)
        {
            Debug.LogError("Dungeon was not built successfully");
        }
    }



    #region Validation
#if UNITY_EDITOR

    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(dungeonLevelList), dungeonLevelList);
    }
#endif
    #endregion
}
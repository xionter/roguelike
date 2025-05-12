using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class GameManager : SingletonMonobehaviour<GameManager>
{
    #region Header GAMEOBJECT REFERENCES
    [Space(10)]
    [Header("GAMEOBJECT REFERENCES")]
    #endregion Header GAMEOBJECT REFERENCES


    #region Tooltip
    [Tooltip("Populate with the FadeImage canvasgroup component in the FadeScreenUI")]
    #endregion Tooltip
    [SerializeField] private CanvasGroup canvasGroup;

    /*
    #region Header DUNGEON LEVELS

    [Space(10)]
    [Header("DUNGEON LEVELS")]
    #endregion Header DUNGEON LEVELS

    #region Tooltip

    [Tooltip("Populate with the dungeon level scriptable objects")]

    #endregion Tooltip
*/
    [SerializeField] private DungeonLevelSO dungeonLevel;


    #region Tooltip

    [Tooltip("Populate with the starting dungeon level for testing , first level = 0")]

    #endregion Tooltip
    private Room currentRoom;
    private Room previousRoom;
    private PlayerDetailsSO playerDetails;
    private Player player;

    [HideInInspector] public GameState gameState;
    [HideInInspector] public GameState previousGameState;
    //private long gameScore;
    //private int scoreMultiplier;
 //   private InstantiatedRoom bossRoom;
    //private bool isFading = false;

    protected override void Awake()
    {
        // Call base class
        base.Awake();

        // Set player details - saved in current player scriptable object from the main menu
        playerDetails = GameResources.Instance.currentPlayer.playerDetails;

        // Instantiate player
        InstantiatePlayer();

    }
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        previousGameState = GameState.gameStarted;
        gameState = GameState.gameStarted;

        //gameScore = 0;

        //scoreMultiplier = 1;

        // Set screen to black
        //StartCoroutine(Fade(0f, 1f, 0f, Color.black));
    }


        /// <summary>
    /// Create player in scene at position
    /// </summary>
    private void InstantiatePlayer()
    {
        // Instantiate player
        GameObject playerGameObject = Instantiate(playerDetails.playerPrefab);

        // Initialize Player
        player = playerGameObject.GetComponent<Player>();

        player.Initialize(playerDetails);

    }

    // Update is called once per frame
    private void Update()
    {
        HandleGameState();
        //тестирование
        if (Input.GetKeyDown(KeyCode.P))
            gameState = GameState.gameStarted;
    }

    private void HandleGameState()
    {
        switch (gameState)
        {
            case GameState.gameStarted:
                PlayDungeonLevel();
                
                gameState = GameState.playingLevel;

                break;
            
        }
    }
    /*
    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
    {
        SetCurrentRoom(roomChangedEventArgs.room);
    }*/


    private void PlayDungeonLevel()
    {
        bool dungeonBuiltSucessfully = DungeonBuilder.Instance.GenerateDungeon(dungeonLevel);

        if (!dungeonBuiltSucessfully)
        {
            Debug.LogError("Couldn't build dungeon from specified rooms and node graphs");
        }

    /*
        // Call static event that room has changed.
        StaticEventHandler.CallRoomChangedEvent(currentRoom);

        // Set player roughly mid-room
        //player.gameObject.transform.position = new Vector3((currentRoom.lowerBounds.x + currentRoom.upperBounds.x) / 2f, (currentRoom.lowerBounds.y + currentRoom.upperBounds.y) / 2f, 0f);

        // Get nearest spawn point in room nearest to player
        //player.gameObject.transform.position = HelperUtilities.GetSpawnPositionNearestToPlayer(player.gameObject.transform.position);

        // Display Dungeon Level Text
        StartCoroutine(DisplayDungeonLevelText());

        //// ** Demo code
        //RoomEnemiesDefeated();*/
    }

    public Player GetPlayer()
    {
        return player;
    }

    public Room GetCurrentRoom()
    {
        return currentRoom;
    }



    public DungeonLevelSO GetCurrentDungeonLevel()
    {
        return dungeonLevel;
        //return dungeonLevel[currentdungeonLevelIndex];
    }

    #region Validation
#if UNITY_EDITOR

    private void OnValidate()
    {
        //HelperUtilities.ValidateCheckEnumerableValues(this, nameof(dungeonLevel), dungeonLevel);
    }
#endif
    #endregion
}






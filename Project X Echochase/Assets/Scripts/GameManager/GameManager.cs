using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
/**/
[DisallowMultipleComponent]
public class GameManager : SingletonMonobehaviour<GameManager>
{
    #region Header GAMEOBJECT REFERENCES
    [Space(10)]
    [Header("ССЫЛКИ НА GAMEOBJECT")]
    #endregion Header GAMEOBJECT REFERENCES


    #region Tooltip

    [Tooltip("Заполнить игровым объектом меню паузы в иерархии")]

    #endregion Tooltip

    [SerializeField] private GameObject pauseMenu;

    #region Tooltip
    [Tooltip("Заполните компонентом CanvasGroup из FadeScreenUI")]
    #endregion Tooltip
    [SerializeField] private CanvasGroup canvasGroup;

    #region Header DUNGEON LEVELS

    [Space(10)]
    [Header("УРОВНИ ПОДЗЕМЕЛЬЯ")]
    #endregion Header DUNGEON LEVELS

    #region Tooltip

    [Tooltip("Заполните объектами ScriptableObject для уровней подземелья")]

    #endregion Tooltip
    [SerializeField] private DungeonLevelSO dungeonLevel;


    #region Tooltip

    [Tooltip("Заполните начальным уровнем подземелья для тестирования, первый уровень = 0")]

    #endregion Tooltip
    private Room currentRoom;
    private Room previousRoom;
    private PlayerDetailsSO playerDetails;
    private Player player;

    [HideInInspector] public GameState gameState;
    [HideInInspector] public GameState previousGameState;
    private long gameScore;
    private int scoreMultiplier;
    private InstantiatedRoom bossRoom;
    private bool isFading = false;

    protected override void Awake()
    {
        // Вызов метода базового класса
        base.Awake();

        // Установить данные игрока - сохранены в текущем ScriptableObject игрока из главного меню
        playerDetails = GameResources.Instance.currentPlayer.playerDetails;

        // Создать игрока
        InstantiatePlayer();

    }


    // Start вызывается один раз перед первым выполнением Update после создания MonoBehaviour
    private void Start()
    {
        previousGameState = GameState.gameStarted;
        gameState = GameState.gameStarted;
    }

    /// <summary>
    /// Установить текущую комнату, в которой находится игрок
    /// </summary>
    public void SetCurrentRoom(Room room)
    {
        previousRoom = currentRoom;
        currentRoom = room;

        //// Отладка
        //Debug.Log(room.prefab.name.ToString());
    }


    /// <summary>
    /// Создать игрока в сцене в указанной позиции
    /// </summary>
    private void InstantiatePlayer()
    {
        // Создать объект игрока
        var playerGameObject = Instantiate(playerDetails.playerPrefab);

        // Инициализировать игрока
        player = playerGameObject.GetComponent<Player>();

        player.Initialize(playerDetails);

    }

    // Update вызывается один раз за кадр
    private void Update()
    {
        HandleGameState();
        //тестирование
        if (Input.GetKeyDown(KeyCode.U))
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

            case GameState.playingLevel:

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    PauseGameMenu();
                }

                break;

            case GameState.engagingEnemies:

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    PauseGameMenu();
                }

                break;

            case GameState.bossStage:

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    PauseGameMenu();
                }

                break;


            case GameState.engagingBoss:

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    PauseGameMenu();
                }
                break;


            case GameState.restartGame:
                RestartGame();
                break;

            case GameState.gamePaused:
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    PauseGameMenu();
                }
                break;

            case GameState.gameWon:
                if (previousGameState != GameState.gameWon)
                    GameWon();
                break;
        }
    }

    private void OnEnable()
    {
        // Подписаться на событие изменения комнаты
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
        StaticEventHandler.OnRoomEnemiesDefeated += StaticEventHandler_OnRoomEnemiesDefeated;
    }

    private void OnDisable()
    {
        // Отписаться от события изменения комнаты
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
        StaticEventHandler.OnRoomEnemiesDefeated -= StaticEventHandler_OnRoomEnemiesDefeated;
    }


    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
    {
        SetCurrentRoom(roomChangedEventArgs.room);
    }
    private void StaticEventHandler_OnRoomEnemiesDefeated(RoomEnemiesDefeatedArgs roomEnemiesDefeatedArgs)
    {
        RoomEnemiesDefeated();
    }

    private void RoomEnemiesDefeated()
    {
        bool isDungeonClearOfRegularEnemies = true;
        bossRoom = null;

        foreach (KeyValuePair<string, Room> keyValuePair in DungeonBuilder.Instance.dungeonBuilderRoomDictionary)
        {
            if (keyValuePair.Value.roomNodeType.isBossRoom)
            {
                bossRoom = keyValuePair.Value.instantiatedRoom;
                continue;
            }

            if (!keyValuePair.Value.isClearedOfEnemies)
            {
                isDungeonClearOfRegularEnemies = false;
                break;
            }
        }

        if ((isDungeonClearOfRegularEnemies && bossRoom == null) || (isDungeonClearOfRegularEnemies && bossRoom.room.isClearedOfEnemies))
        {
            gameState = GameState.gameWon;
        }
        else if (isDungeonClearOfRegularEnemies)
        {
            gameState = GameState.bossStage;
        }
    }



    private void PlayDungeonLevel()
    {
        // Проверяем успешность генерации подземелья
        var dungeonBuiltSuccessfully = DungeonBuilder.Instance.GenerateDungeon(dungeonLevel);

        if (!dungeonBuiltSuccessfully)
        {   
            Debug.LogError("Не удалось построить подземелье из указанных комнат и графов узлов");
            return; 
        }

        // Убедимся, что текущая комната не null
        if (currentRoom == null)
        {
            return;
        }

        // Вызвать статическое событие, что комната изменилась
        StaticEventHandler.CallRoomChangedEvent(currentRoom);

        // Проверяем, что игрок создан
        if (player == null || player.gameObject == null)
        {
            Debug.LogError("Игрок не инициализирован.");
            return;
        }

        // Установить игрока примерно в центре комнаты
        player.gameObject.transform.position = new Vector3(
            (currentRoom.lowerBounds.x + currentRoom.upperBounds.x) / 2f,
            (currentRoom.lowerBounds.y + currentRoom.upperBounds.y) / 2f,
            0f
        );

        // Получить ближайшую точку появления в комнате, ближайшую к игроку
        player.gameObject.transform.position = HelperUtilities.GetSpawnPositionNearestToPlayer(player.gameObject.transform.position);
    }

    public Player GetPlayer()
    {
        return player;
    }

    public Sprite GetPlayerMiniMapIcon()
    {
        return playerDetails.playerMiniMapIcon;
    }

    public Room GetCurrentRoom()
    {
        return currentRoom;
    }
    
    public DungeonLevelSO GetCurrentDungeonLevel()
    {
        return dungeonLevel;
    }

    private void GameWon()
    {
        previousGameState = GameState.gameWon;

        GetPlayer().playerControl.DisablePlayer();
        gameState = GameState.restartGame;
    }

    private void RestartGame()
    {
        SceneManager.LoadScene("MainMenuScene");
    }


    public void PauseGameMenu()
    {
        if (gameState != GameState.gamePaused)
        {
            pauseMenu.SetActive(true);
            GetPlayer().playerControl.DisablePlayer();

            previousGameState = gameState;
            gameState = GameState.gamePaused;
        }
        else if (gameState == GameState.gamePaused)
        {
            pauseMenu.SetActive(false);
            GetPlayer().playerControl.EnablePlayer();

            gameState = previousGameState;
            previousGameState = GameState.gamePaused;

        }
    }



        #region Validation

#if UNITY_EDITOR

    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(pauseMenu), pauseMenu);
        //HelperUtilities.ValidateCheckNullValue(this, nameof(messageTextTMP), messageTextTMP);
        HelperUtilities.ValidateCheckNullValue(this, nameof(canvasGroup), canvasGroup);
    }

#endif

    #endregion Validation


}
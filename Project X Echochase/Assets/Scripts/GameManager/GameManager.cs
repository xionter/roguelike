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
        GameObject playerGameObject = Instantiate(playerDetails.playerPrefab);

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

        }
    }

    private void OnEnable()
    {
        // Подписаться на событие изменения комнаты
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
    }

    private void OnDisable()
    {
        // Отписаться от события изменения комнаты
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
    }


    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
    {
        SetCurrentRoom(roomChangedEventArgs.room);
    }


    private void PlayDungeonLevel()
    {
        bool dungeonBuiltSucessfully = DungeonBuilder.Instance.GenerateDungeon(dungeonLevel);

        if (!dungeonBuiltSucessfully)
        {
            Debug.LogError("Не удалось построить подземелье из указанных комнат и графов узлов");
        }


        // Вызвать статическое событие, что комната изменилась
        StaticEventHandler.CallRoomChangedEvent(currentRoom);

        // Установить игрока примерно в центре комнаты
        // player.gameObject.transform.position = new Vector3((currentRoom.lowerBounds.x + currentRoom.upperBounds.x) / 2f, (currentRoom.lowerBounds.y + currentRoom.upperBounds.y) / 2f, 0f);

        // Получить ближайшую точку появления в комнате, ближайшую к игроку
        // player.gameObject.transform.position = HelperUtilities.GetSpawnPositionNearestToPlayer(player.gameObject.transform.position);
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
    }
}
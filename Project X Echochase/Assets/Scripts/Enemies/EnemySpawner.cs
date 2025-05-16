//НЕПОНЯТНО, ПОЧЕМУ НЕ РАБОТАЕТ. НЕ УДАЛЯТЬ!!!!!



//using System.Collections;
//using UnityEngine;

//[DisallowMultipleComponent]
//public class EnemySpawner : SingletonMonobehaviour<EnemySpawner>
//{
//    private int enemiesToSpawn;
//    private int currentEnemyCount;
//    private int enemiesSpawnedSoFar;
//    private int enemyMaxConcurrentSpawnNumber;
//    private Room currentRoom;
//    private RoomEnemySpawnParameters roomEnemySpawnParameters;

//    private void OnEnable()
//    {
//        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;
//    }

//    private void OnDisable()
//    {
//        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;
//    }

//    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
//    {
//        enemiesSpawnedSoFar = 0;
//        currentEnemyCount = 0;

//        currentRoom = roomChangedEventArgs.room;

//        //MusicManager.Instance.PlayMusic(currentRoom.ambientMusic, 0.2f, 2f);

//        // если комната - корридор или вход, то выходим
//        if (currentRoom.roomNodeType.isCorridorEW || currentRoom.roomNodeType.isCorridorNS || currentRoom.roomNodeType.isEntrance)
//            return;

//        // если комната уже была зачищена, то выходим
//        if (currentRoom.isClearedOfEnemies) return;

//        enemiesToSpawn = currentRoom.GetNumberOfEnemiesToSpawn(GameManager.Instance.GetCurrentDungeonLevel());

//        roomEnemySpawnParameters = currentRoom.GetRoomEnemySpawnParameters(GameManager.Instance.GetCurrentDungeonLevel());

//        if (enemiesToSpawn == 0)
//        {
//            currentRoom.isClearedOfEnemies = true;

//            return;
//        }

//        enemyMaxConcurrentSpawnNumber = GetConcurrentEnemies();

//        //MusicManager.Instance.PlayMusic(currentRoom.battleMusic, 0.2f, 0.5f);

//        currentRoom.instantiatedRoom.LockDoors();

//        SpawnEnemies();
//    }

//    private void SpawnEnemies()
//    {
//        // если gameState - босс
//        if (GameManager.Instance.gameState == GameState.bossStage)
//        {
//            GameManager.Instance.previousGameState = GameState.bossStage;
//            GameManager.Instance.gameState = GameState.engagingBoss;
//        }

//        // если gameState - враги
//        else if(GameManager.Instance.gameState == GameState.playingLevel)
//        {
//            GameManager.Instance.previousGameState = GameState.playingLevel;
//            GameManager.Instance.gameState = GameState.engagingEnemies;
//        }

//        StartCoroutine(SpawnEnemiesRoutine());
//    }

//    private IEnumerator SpawnEnemiesRoutine()
//    {
//        Grid grid = currentRoom.instantiatedRoom.grid;

//        RandomSpawnableObject<EnemyDetailsSO> randomEnemyHelperClass = new RandomSpawnableObject<EnemyDetailsSO>(currentRoom.enemiesByLevelList);

//        if (currentRoom.spawnPositionArray.Length > 0)
//        {
//            for (var i = 0; i < enemiesToSpawn; ++i)
//            {
//                while (currentEnemyCount >= enemyMaxConcurrentSpawnNumber)
//                {
//                    yield return null;
//                }

//                Vector3Int cellPosition = (Vector3Int)currentRoom.spawnPositionArray[Random.Range(0, currentRoom.spawnPositionArray.Length)];

//                CreateEnemy(randomEnemyHelperClass.GetItem(), grid.CellToWorld(cellPosition));

//                yield return new WaitForSeconds(GetEnemySpawnInterval());
//            }
//        }
//    }


//    private float GetEnemySpawnInterval()
//    {
//        return (Random.Range(roomEnemySpawnParameters.minSpawnInterval, roomEnemySpawnParameters.maxSpawnInterval));
//    }

//    private int GetConcurrentEnemies()
//    {
//        return (Random.Range(roomEnemySpawnParameters.minConcurrentEnemies, roomEnemySpawnParameters.maxConcurrentEnemies));
//    }

//    /// <summary>
//    /// создать врага в указанной позиции
//    /// </summary>
//    private void CreateEnemy(EnemyDetailsSO enemyDetails, Vector3 position)
//    {
//        enemiesSpawnedSoFar++;

//        currentEnemyCount++;

//        DungeonLevelSO dungeonLevel = GameManager.Instance.GetCurrentDungeonLevel();

//        GameObject enemy = Instantiate(enemyDetails.enemyPrefab, position, Quaternion.identity, transform);

//        enemy.GetComponent<Enemy>().EnemyInitialization(enemyDetails, enemiesSpawnedSoFar, dungeonLevel);

//        enemy.GetComponent<DestroyedEvent>().OnDestroyed += Enemy_OnDestroyed;
//    }

//    private void Enemy_OnDestroyed(DestroyedEvent destroyedEvent, DestroyedEventArgs destroyedEventArgs)
//    {
//        destroyedEvent.OnDestroyed -= Enemy_OnDestroyed;

//        currentEnemyCount--;

////       StaticEventHandler.CallPointsScoredEvent(destroyedEventArgs.points);

//        if (currentEnemyCount <= 0 && enemiesSpawnedSoFar == enemiesToSpawn)
//        {
//            currentRoom.isClearedOfEnemies = true;

//            // устанавливаем gameState
//            if (GameManager.Instance.gameState == GameState.engagingEnemies)
//            {
//                GameManager.Instance.gameState = GameState.playingLevel;
//                GameManager.Instance.previousGameState = GameState.engagingEnemies;
//            }

//            else if (GameManager.Instance.gameState == GameState.engagingBoss)
//            {
//                GameManager.Instance.gameState = GameState.bossStage;
//                GameManager.Instance.previousGameState = GameState.engagingBoss;
//            }

//            // разблокировать двери
//            currentRoom.instantiatedRoom.UnlockDoors(Settings.doorUnlockDelay);

//            // изменить музыку
//            //MusicManager.Instance.PlayMusic(currentRoom.ambientMusic, 0.2f, 2f);

//            // enemies defeated event
//            StaticEventHandler.CallRoomEnemiesDefeatedEvent(currentRoom);
//        }
//    }

//}

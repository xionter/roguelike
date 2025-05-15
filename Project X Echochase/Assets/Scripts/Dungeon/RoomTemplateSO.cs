using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Room_", menuName = "Scriptable Objects/Dungeon/Room")]
public class RoomTemplateSO : ScriptableObject
{
    public string guid;

    #region Header ROOM PREFAB

    [Space(10)]
    [Header("ПРЕФАБ КОМНАТЫ")]

    #endregion Header ROOM PREFAB

    #region Tooltip

    [Tooltip("Префаб игрового объекта для комнаты (он будет содержать все тайлмапы для комнаты и объектов окружения)")]

    #endregion Tooltip

    public GameObject prefab;

    [HideInInspector] public GameObject previousPrefab; // используется для повторной генерации GUID, если объект был скопирован и префаб изменился

    #region Header ROOM CONFIGURATION

    [Space(10)]
    [Header("КОНФИГУРАЦИЯ КОМНАТЫ")]

    #endregion Header ROOM CONFIGURATION

    #region Tooltip

    [Tooltip("Тип узла комнаты. Типы узлов комнаты соответствуют узлам комнаты в графе узлов комнаты. Исключением являются коридоры. В графе узлов комнаты есть только один тип коридора 'Corridor'. Для шаблонов комнат есть один тип узла коридора - CorridorNS.")]

    #endregion Tooltip

    public RoomNodeTypeSO roomNodeType;

    #region Tooltip

    [Tooltip("Если представить прямоугольник вокруг тайлмапа комнаты, который полностью её охватывает, нижняя граница комнаты представляет нижний левый угол этого прямоугольника. Это значение должно быть определено из тайлмапа комнаты (с использованием указателя координат кисти для получения позиции сетки тайлмапа для этого нижнего левого угла (Примечание: это локальная позиция тайлмапа, а НЕ мировая позиция)).")]

    #endregion Tooltip

    public Vector2Int lowerBounds;

    #region Tooltip

    [Tooltip("Если представить прямоугольник вокруг тайлмапа комнаты, который полностью её охватывает, верхняя граница комнаты представляет верхний правый угол этого прямоугольника. Это значение должно быть определено из тайлмапа комнаты (с использованием указателя координат кисти для получения позиции сетки тайлмапа для этого верхнего правого угла (Примечание: это локальная позиция тайлмапа, а НЕ мировая позиция)).")]

    #endregion Tooltip

    public Vector2Int upperBounds;

    #region Tooltip

    [Tooltip("В комнате должно быть максимум два дверных проёма - один на север и один на запад. Они должны иметь одинаковый размер проёма в 3 тайла, при этом средняя позиция тайла является координатой 'позиции' дверного проёма.")]

    #endregion Tooltip

    [SerializeField] public List<DoorWay> doorwayList;

    #region Tooltip

    [Tooltip("Каждая возможная позиция спавна (используется для врагов и сундуков) для комнаты в координатах тайлмапа должна быть добавлена в этот массив")]

    #endregion Tooltip

    public Vector2Int[] spawnPositionArray;

    #region Header ENEMY DETAILS

    [Space(10)]
    [Header("ДЕТАЛИ ВРАГОВ")]

    #endregion Header ENEMY DETAILS

    #region Tooltip

    [Tooltip("Заполните список всеми врагами, которые могут быть заспавнены в этой комнате по уровням подземелья, включая соотношение (случайное) этого типа врагов, которые будут заспавнены")]

    #endregion Tooltip

    public List<SpawnableObjectsByLevel<EnemyDetailsSO>> enemiesByLevelList;


    #region Tooltip

    [Tooltip("Заполните список параметрами спавна для врагов.")]

    #endregion Tooltip

    public List<RoomEnemySpawnParameters> roomEnemySpawnParametersList;

    /// <summary>
    /// Возвращает список входов для шаблона комнаты
    /// </summary>
    public List<DoorWay> GetDoorwayList()
    {
        return doorwayList;
    }

    #region Validation

#if UNITY_EDITOR

    // Проверка полей ScriptableObject
    private void OnValidate()
    {
        // Установить уникальный id, если он пуст или префаб изменился
        if (guid == "" || previousPrefab != prefab)
        {
            guid = GUID.Generate().ToString();
            previousPrefab = prefab;
            EditorUtility.SetDirty(this); // пометить объект как изменённый
        }

        HelperUtilities.ValidateCheckNullValue(this, nameof(prefab), prefab);



        if (enemiesByLevelList.Count > 0 || roomEnemySpawnParametersList.Count > 0)
        {
            HelperUtilities.ValidateCheckEnumerableValues(this, nameof(enemiesByLevelList), enemiesByLevelList);
            HelperUtilities.ValidateCheckEnumerableValues(this, nameof(roomEnemySpawnParametersList), roomEnemySpawnParametersList);

            foreach (RoomEnemySpawnParameters roomEnemySpawnParameters in roomEnemySpawnParametersList)
            {
                HelperUtilities.ValidateCheckNullValue(this, nameof(roomEnemySpawnParameters.dungeonLevel), roomEnemySpawnParameters.dungeonLevel);

                HelperUtilities.ValidateCheckPositiveRange(this, nameof(roomEnemySpawnParameters.minTotalEnemiesToSpawn),
                  roomEnemySpawnParameters.minTotalEnemiesToSpawn, nameof(roomEnemySpawnParameters.maxTotalEnemiesToSpawn),
                  roomEnemySpawnParameters.maxTotalEnemiesToSpawn, true);

                HelperUtilities.ValidateCheckPositiveRange(this, nameof(roomEnemySpawnParameters.minSpawnInterval),
                  roomEnemySpawnParameters.minSpawnInterval, nameof(roomEnemySpawnParameters.maxSpawnInterval),
                  roomEnemySpawnParameters.maxSpawnInterval, true);

                HelperUtilities.ValidateCheckPositiveRange(this, nameof(roomEnemySpawnParameters.minConcurrentEnemies),
                  roomEnemySpawnParameters.minConcurrentEnemies, nameof(roomEnemySpawnParameters.maxConcurrentEnemies),
                  roomEnemySpawnParameters.maxConcurrentEnemies, false);

                bool isEnemyTypesListForDungeonLevel = false;

                // проверка типов врагов
                foreach (SpawnableObjectsByLevel<EnemyDetailsSO> dungeonObjectsByLevel in enemiesByLevelList)
                {
                    if (dungeonObjectsByLevel.dungeonLevel == roomEnemySpawnParameters.dungeonLevel && dungeonObjectsByLevel.spawnableObjectRatioList.Count > 0)
                        isEnemyTypesListForDungeonLevel = true;

                    HelperUtilities.ValidateCheckNullValue(this, nameof(dungeonObjectsByLevel.dungeonLevel), dungeonObjectsByLevel.dungeonLevel);

                    foreach (SpawnableObjectRatio<EnemyDetailsSO> dungeonObjectRatio in dungeonObjectsByLevel.spawnableObjectRatioList)
                    {
                        HelperUtilities.ValidateCheckNullValue(this, nameof(dungeonObjectRatio.dungeonObject), dungeonObjectRatio.dungeonObject);

                        HelperUtilities.ValidateCheckPositiveValue(this, nameof(dungeonObjectRatio.ratio), dungeonObjectRatio.ratio, false);
                    }

                }

                if (isEnemyTypesListForDungeonLevel == false && roomEnemySpawnParameters.dungeonLevel != null)
                {
                    Debug.Log("Не указаны типы врагов для уровня подземелья " + roomEnemySpawnParameters.dungeonLevel.levelName + " в игровом объекте " + this.name.ToString());
                }
            }
        }

        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(doorwayList), doorwayList);

        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(spawnPositionArray), spawnPositionArray);
    }

#endif

    #endregion Validation
}
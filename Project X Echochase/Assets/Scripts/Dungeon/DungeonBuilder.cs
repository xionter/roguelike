using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.Tilemaps;

[DisallowMultipleComponent]
public class DungeonBuilder : SingletonMonobehaviour<DungeonBuilder>
{
    public Dictionary<string, Room> dungeonBuilderRoomDictionary = new Dictionary<string, Room>();
    private Dictionary<string, RoomTemplateSO> roomTemplateDictionary = new Dictionary<string, RoomTemplateSO>();
    private List<RoomTemplateSO> roomTemplateList;
    private RoomNodeTypeListSO roomNodeTypeList;
    private bool dungeonBuildSuccessful;
    [Header("Резервный шаблон")]
    [SerializeField] private RoomTemplateSO defaultRoomTemplate;

    private DungeonLevelSO dungeonLevel;

    private void OnEnable()
    {
        GameResources.Instance.dimmedMaterial.SetFloat("Alpha_Slider", 0f);
    }

    private void OnDisable()
    {
        GameResources.Instance.dimmedMaterial.SetFloat("Alpha_Slider", 1f);
    }
    
    protected override void Awake()
    {
        base.Awake();
        
        LoadRoomNodeTypeList();
    }
    
    private void LoadRoomNodeTypeList()
    {
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }
    
    public bool GenerateDungeon(DungeonLevelSO currentDungeonLevel)
    {
        roomTemplateList = currentDungeonLevel.roomTemplateList;
        dungeonLevel = currentDungeonLevel;
        LoadRoomTemplatesIntoDictionary();

        dungeonBuildSuccessful = false;
        int dungeonBuildAttempts = 0;

        // Пытаемся построить подземелье, пока не получится или не достигнем максимального количества попыток
        while (!dungeonBuildSuccessful && dungeonBuildAttempts < Settings.maxDungeonBuildAttempts)
        {
            dungeonBuildAttempts++;

            RoomNodeGraphSO roomNodeGraph = GetRandomRoomNodeGraph(currentDungeonLevel.roomNodeGraph);
            
            int dungeonRebuildAttemptsForNodeGraph = 0;
            dungeonBuildSuccessful = false;

            // Пытаемся построить подземелье для текущего графа комнат
            while (!dungeonBuildSuccessful && dungeonRebuildAttemptsForNodeGraph <= Settings.maxDungeonRebuildAttemptsForRoomGraph)
            {

                ClearDungeon();

                dungeonRebuildAttemptsForNodeGraph++;

                dungeonBuildSuccessful = AttemptToBuildRandomDungeon(roomNodeGraph);
            }

            // Если удалось построить подземелье, создаём игровые объекты комнат
            if (dungeonBuildSuccessful)
            {
                InstantiateRoomGameobjects();
            }
        }

        return dungeonBuildSuccessful;
    }
    
    private void LoadRoomTemplatesIntoDictionary()
    {
        roomTemplateDictionary.Clear();

        // Загружаем шаблоны комнат в словарь
        foreach (RoomTemplateSO roomTemplate in roomTemplateList)
        {
            if (!roomTemplateDictionary.ContainsKey(roomTemplate.guid))
            {
                roomTemplateDictionary.Add(roomTemplate.guid, roomTemplate);
            }
            else
            {
                Debug.Log("Дублирующийся ключ шаблона комнаты в " + roomTemplateList);
            }
        }
    }
    
    private bool AttemptToBuildRandomDungeon(RoomNodeGraphSO roomNodeGraph)
    {
        // Создаём очередь для обработки комнат
        Queue<RoomNodeSO> openRoomNodeQueue = new Queue<RoomNodeSO>();

        // Добавляем начальную комнату
        RoomNodeSO entranceNode = roomNodeGraph.GetRoomNode(roomNodeTypeList.list.Find(x => x.isEntrance));

        if (entranceNode != null)
        {
            openRoomNodeQueue.Enqueue(entranceNode);
        }
        else
        {
            Debug.Log("Нет начальной комнаты");
            return false;  
        }
        
        bool noRoomOverlaps = true;
        
        noRoomOverlaps = ProcessRoomsInOpenRoomNodeQueue(roomNodeGraph, openRoomNodeQueue, noRoomOverlaps);

        // Если все комнаты добавлены и нет пересечений
        if (openRoomNodeQueue.Count == 0 && noRoomOverlaps)
        {
            return true;
        }

        return false;
    }
    
    private bool ProcessRoomsInOpenRoomNodeQueue(RoomNodeGraphSO roomNodeGraph, Queue<RoomNodeSO> openRoomNodeQueue, bool noRoomOverlaps)
    {
        // Пока в очереди есть комнаты и не обнаружено пересечений
        while (openRoomNodeQueue.Count > 0 && noRoomOverlaps == true)
        {
            // Получаем следующую комнату из очереди
            RoomNodeSO roomNode = openRoomNodeQueue.Dequeue();

            // Добавляем дочерние узлы в очередь из графа комнат (связанные с этим родительским узлом)
            foreach (RoomNodeSO childRoomNode in roomNodeGraph.GetChildRoomNodes(roomNode))
            {
                openRoomNodeQueue.Enqueue(childRoomNode);
            }

            // Если комната является входом, отмечаем её как размещённую и добавляем в словарь комнат
            if (roomNode.roomNodeType.isEntrance)
            {
                RoomTemplateSO roomTemplate = GetRandomRoomTemplate(roomNode.roomNodeType);
                if (roomTemplate == null)
                {
                    Debug.LogWarning($"[{nameof(DungeonBuilder)}] Нет шаблона для {roomNode.roomNodeType.name}, подставляю defaultRoomTemplate");
                    roomTemplate = defaultRoomTemplate;
                }

                Room room = CreateRoomFromRoomTemplate(roomTemplate, roomNode);

                room.isPositioned = true;

                dungeonBuilderRoomDictionary.Add(room.id, room);
            }
            // Если тип комнаты не является входом
            else
            {
                // Получаем родительскую комнату для узла
                Room parentRoom = dungeonBuilderRoomDictionary[roomNode.parentRoomNodeIDList[0]];

                // Проверяем, можно ли разместить комнату без пересечений
                noRoomOverlaps = CanPlaceRoomWithNoOverlaps(roomNode, parentRoom);
            }
        }

        return noRoomOverlaps;
    }
    
    private bool CanPlaceRoomWithNoOverlaps(RoomNodeSO roomNode, Room parentRoom)
    {
        bool roomOverlaps = true;
        
        while (roomOverlaps)
        {
            // Получаем список доступных дверных проёмов родительской комнаты
            List<DoorWay> unconnectedAvailableParentDoorways = GetUnconnectedAvailableDoorways(parentRoom.doorWayList).ToList();

            if (unconnectedAvailableParentDoorways.Count == 0)
            {
                return false; 
            }

            DoorWay doorwayParent = unconnectedAvailableParentDoorways[UnityEngine.Random.Range(0, unconnectedAvailableParentDoorways.Count)];

            // Получаем случайный шаблон комнаты, соответствующий родительской комнате
            RoomTemplateSO roomtemplate = GetRandomTemplateForRoomConsistentWithParent(roomNode, doorwayParent);

            Room room = CreateRoomFromRoomTemplate(roomtemplate, roomNode);

            // Если удалось разместить комнату
            if (PlaceTheRoom(parentRoom, doorwayParent, room))
            {
                roomOverlaps = false;

                room.isPositioned = true;

                dungeonBuilderRoomDictionary.Add(room.id, room);
            }
            else
            {
                roomOverlaps = true;
            }
        }

        return true;  // Нет пересечений комнат
    }
    
    private RoomTemplateSO GetRandomTemplateForRoomConsistentWithParent(RoomNodeSO roomNode, DoorWay doorwayParent)
    {
        RoomTemplateSO roomtemplate = null;

        // Если узел комнаты является коридором, выбираем случайный подходящий шаблон коридора на основе ориентации дверного проёма
        if (roomNode.roomNodeType.isCorridor)
        {
            switch (doorwayParent.orientation)
            {
                case Orientation.north:
                case Orientation.south:
                    roomtemplate = GetRandomRoomTemplate(roomNodeTypeList.list.Find(x => x.isCorridorNS));
                    break;

                case Orientation.east:
                case Orientation.west:
                    roomtemplate = GetRandomRoomTemplate(roomNodeTypeList.list.Find(x => x.isCorridorEW));
                    break;

                case Orientation.none:
                    break;

                default:
                    break;
            }
        }
        // В противном случае выбираем случайный шаблон комнаты
        else
        {
            roomtemplate = GetRandomRoomTemplate(roomNode.roomNodeType);
        }

        return roomtemplate;
    }
    
    private bool PlaceTheRoom(Room parentRoom, DoorWay doorwayParent, Room room)
    {
        // Получаем текущую позицию дверного проёма комнаты
        DoorWay doorway = GetOppositeDoorway(doorwayParent, room.doorWayList);

        // Возвращаемся, если в комнате нет дверного проёма, противоположного родительскому
        if (doorway == null)
        {
            // Просто отмечаем родительский дверной проём как недоступный, чтобы не пытаться подключить его снова
            doorwayParent.isUnavailable = true;

            return false;
        }

        // Вычисляем позицию родительского дверного проёма в "мировых" координатах
        Vector2Int parentDoorwayPosition = parentRoom.lowerBounds + doorwayParent.position - parentRoom.templateLowerBounds;

        Vector2Int adjustment = Vector2Int.zero;

        // Вычисляем смещение позиции на основе ориентации дверного проёма комнаты, которую пытаемся подключить
        switch (doorway.orientation)
        {
            case Orientation.north:
                adjustment = new Vector2Int(0, -1);
                break;

            case Orientation.east:
                adjustment = new Vector2Int(-1, 0);
                break;

            case Orientation.south:
                adjustment = new Vector2Int(0, 1);
                break;

            case Orientation.west:
                adjustment = new Vector2Int(1, 0);
                break;

            case Orientation.none:
                break;

            default:
                break;
        }

        // Вычисляем нижние и верхние границы комнаты на основе позиции, чтобы выровнять её с родительским дверным проёмом
        room.lowerBounds = parentDoorwayPosition + adjustment + room.templateLowerBounds - doorway.position;
        room.upperBounds = room.lowerBounds + room.templateUpperBounds - room.templateLowerBounds;

        Room overlappingRoom = CheckForRoomOverlap(room);

        if (overlappingRoom == null)
        {
            // Отмечаем дверные проёмы как подключённые и недоступные
            doorwayParent.isConnected = true;
            doorwayParent.isUnavailable = true;

            doorway.isConnected = true;
            doorway.isUnavailable = true;

            // Возвращаем true, чтобы показать, что комнаты подключены без пересечений
            return true;
        }
        else
        {
            // Просто отмечаем родительский дверной проём как недоступный, чтобы не пытаться подключить его снова
            doorwayParent.isUnavailable = true;

            return false;
        }
    }
    
    private DoorWay GetOppositeDoorway(DoorWay parentDoorway, List<DoorWay> doorwayList)
    {
        // Ищем противоположный дверной проём
        foreach (DoorWay doorwayToCheck in doorwayList)
        {
            if (parentDoorway.orientation == Orientation.east && doorwayToCheck.orientation == Orientation.west)
            {
                return doorwayToCheck;
            }
            else if (parentDoorway.orientation == Orientation.west && doorwayToCheck.orientation == Orientation.east)
            {
                return doorwayToCheck;
            }
            else if (parentDoorway.orientation == Orientation.north && doorwayToCheck.orientation == Orientation.south)
            {
                return doorwayToCheck;
            }
            else if (parentDoorway.orientation == Orientation.south && doorwayToCheck.orientation == Orientation.north)
            {
                return doorwayToCheck;
            }
        }

        return null;
    }
    
    private Room CheckForRoomOverlap(Room roomToTest)
    {
        // Проверяем, пересекается ли комната с другими
        foreach (KeyValuePair<string, Room> keyvaluepair in dungeonBuilderRoomDictionary)
        {
            Room room = keyvaluepair.Value;

            if (room.id == roomToTest.id || !room.isPositioned)
                continue;

            if (IsOverLappingRoom(roomToTest, room))
            {
                return room;
            }
        }

        return null;
    }
    
    private bool IsOverLappingRoom(Room room1, Room room2)
    {
        // Проверяем пересечение по оси X
        bool isOverlappingX = IsOverLappingInterval(room1.lowerBounds.x, room1.upperBounds.x, room2.lowerBounds.x, room2.upperBounds.x);

        // Проверяем пересечение по оси Y
        bool isOverlappingY = IsOverLappingInterval(room1.lowerBounds.y, room1.upperBounds.y, room2.lowerBounds.y, room2.upperBounds.y);

        if (isOverlappingX && isOverlappingY)
        {
            return true;
        }
        return false;
    }
    
    private bool IsOverLappingInterval(int imin1, int imax1, int imin2, int imax2)
    {
        // Проверяем пересечение интервалов
        if (Mathf.Max(imin1, imin2) <= Mathf.Min(imax1, imax2))
        {
            return true;
        }

        return false;
    }
    
    private RoomTemplateSO GetRandomRoomTemplate(RoomNodeTypeSO roomNodeType)
    {
        List<RoomTemplateSO> matchingRoomTemplateList = new List<RoomTemplateSO>();

        // Перебираем список шаблонов комнат
        foreach (RoomTemplateSO roomTemplate in roomTemplateList)
        {
            // Добавляем подходящие шаблоны комнат
            if (roomTemplate.roomNodeType == roomNodeType)
            {
                matchingRoomTemplateList.Add(roomTemplate);
            }
        }

        // Возвращаем null, если список пуст
        if (matchingRoomTemplateList.Count == 0)
            return null;

        // Выбираем случайный шаблон комнаты из списка и возвращаем его
        return matchingRoomTemplateList[UnityEngine.Random.Range(0, matchingRoomTemplateList.Count)];
    }
    
    private IEnumerable<DoorWay> GetUnconnectedAvailableDoorways(List<DoorWay> roomDoorwayList)
    {
        // Перебираем список дверных проёмов
        foreach (DoorWay doorway in roomDoorwayList)
        {
            if (!doorway.isConnected && !doorway.isUnavailable)
                yield return doorway;
        }
    }
    
    private Room CreateRoomFromRoomTemplate(RoomTemplateSO roomTemplate, RoomNodeSO roomNode)
    {
        // Инициализируем комнату из шаблона
        Room room = new Room();
        
        room.templateID = roomTemplate.guid;
        room.id = roomNode.id;
        room.prefab = roomTemplate.prefab;
        room.roomNodeType = roomTemplate.roomNodeType;
        room.lowerBounds = roomTemplate.lowerBounds;
        room.upperBounds = roomTemplate.upperBounds;
        room.spawnPositionArray = roomTemplate.spawnPositionArray;
        room.templateLowerBounds = roomTemplate.lowerBounds;
        room.templateUpperBounds = roomTemplate.upperBounds;
        
        room.childRoomIDList = CopyStringList(roomNode.childRoomNodeIDList);
        room.doorWayList = CopyDoorwayList(roomTemplate.doorwayList);

        if (roomNode.parentRoomNodeIDList.Count == 0) 
        {
            room.parentRoomID = "";
            room.isPreviouslyVisited = true;
        }
        else
        {
            room.parentRoomID = roomNode.parentRoomNodeIDList[0];
        }

        return room;
    }
    
    private RoomNodeGraphSO GetRandomRoomNodeGraph(RoomNodeGraphSO roomNodeGraph)
    {
        // Возвращаем граф узлов комнат
        roomNodeGraph = ScriptableObject.CreateInstance<RoomNodeGraphSO>();
        string path = $"Assets/ScriptableObjectAssets/Dungeon/Level_Graph.asset";
        if (string.IsNullOrEmpty(AssetDatabase.GetAssetPath(roomNodeGraph)))
        {
            AssetDatabase.CreateAsset(roomNodeGraph, path);
            AssetDatabase.SaveAssets();
        }
        ProceduralDungeonGenerator.GenerateDungeon(roomNodeGraph, 10, ProceduralDungeonGenerator.Difficulty.Medium);
        return roomNodeGraph;
    }
    
    private List<DoorWay> CopyDoorwayList(List<DoorWay> oldDoorwayList)
    {
        List<DoorWay> newDoorwayList = new List<DoorWay>();

        foreach (DoorWay doorway in oldDoorwayList)
        {
            DoorWay newDoorway = new DoorWay();

            newDoorway.position = doorway.position;
            newDoorway.orientation = doorway.orientation;
            newDoorway.doorPrefab = doorway.doorPrefab;
            newDoorway.isConnected = doorway.isConnected;
            newDoorway.isUnavailable = doorway.isUnavailable;
            newDoorway.doorwayStartCopyPosition = doorway.doorwayStartCopyPosition;
            newDoorway.doorwayCopyTileWidth = doorway.doorwayCopyTileWidth;
            newDoorway.doorwayCopyTileHeight = doorway.doorwayCopyTileHeight;

            newDoorwayList.Add(newDoorway);
        }

        return newDoorwayList;
    }
    
    private List<string> CopyStringList(List<string> oldStringList)
    {
        List<string> newStringList = new List<string>();

        foreach (string stringValue in oldStringList)
        {
            newStringList.Add(stringValue);
        }

        return newStringList;
    }
    
    private void InstantiateRoomGameobjects()
    {
        // Перебираем все комнаты подземелья
        foreach (KeyValuePair<string, Room> keyvaluepair in dungeonBuilderRoomDictionary)
        {
            Room room = keyvaluepair.Value;

            // Вычисляем позицию комнаты (учитываем смещение нижних границ шаблона комнаты)
            Vector3 roomPosition = new Vector3(room.lowerBounds.x - room.templateLowerBounds.x, room.lowerBounds.y - room.templateLowerBounds.y, 0f);

            // Создаём экземпляр комнаты
            GameObject roomGameobject = Instantiate(room.prefab, roomPosition, Quaternion.identity, transform);

            // Получаем компонент InstantiatedRoom из созданного экземпляра
            InstantiatedRoom instantiatedRoom = roomGameobject.GetComponentInChildren<InstantiatedRoom>();

            instantiatedRoom.room = room;

            // Инициализируем созданную комнату
            instantiatedRoom.Initialise(roomGameobject);

            // Сохраняем ссылку на игровой объект
            room.instantiatedRoom = instantiatedRoom;
        }
    }
    
    public RoomTemplateSO GetRoomTemplate(string roomTemplateID)
    {
        if (roomTemplateDictionary.TryGetValue(roomTemplateID, out RoomTemplateSO roomTemplate))
        {
            return roomTemplate;
        }
        else
        {
            return null;
        }
    }
    
    public Room GetRoomByRoomID(string roomID)
    {
        if (dungeonBuilderRoomDictionary.TryGetValue(roomID, out Room room))
        {
            return room;
        }
        else
        {
            return null;
        }
    }
    
    private void ClearDungeon()
    {
        if (dungeonBuilderRoomDictionary.Count > 0)
        {
            foreach (KeyValuePair<string, Room> keyvaluepair in dungeonBuilderRoomDictionary)
            {
                Room room = keyvaluepair.Value;

                if (room.instantiatedRoom != null)
                {
                    Destroy(room.instantiatedRoom.gameObject);
                }
            }

            dungeonBuilderRoomDictionary.Clear();
        }
    }
}
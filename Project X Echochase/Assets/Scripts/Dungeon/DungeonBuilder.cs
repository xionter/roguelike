using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Singleton responsible for building procedural dungeons based on a RoomNodeGraphSO.
/// </summary>
[DisallowMultipleComponent]
public class DungeonBuilder : SingletonMonobehaviour<DungeonBuilder>
{
    // Все сгенерированные в ходе сборки комнаты
    private Dictionary<string, Room> dungeonBuilderRoomDictionary = new Dictionary<string, Room>();
    // Шаблоны комнат, доступные для текущего уровня
    private Dictionary<string, RoomTemplateSO> roomTemplateDictionary = new Dictionary<string, RoomTemplateSO>();
    private List<RoomTemplateSO> roomTemplateList;
    private RoomNodeTypeListSO roomNodeTypeList;
    private DungeonLevelSO dungeonLevel;

    [Header("=== Fallback Template ===")]
    [SerializeField] private RoomTemplateSO defaultRoomTemplate;

    protected override void Awake()
    {
        base.Awake();
        // Получаем список типов нод из GameResources
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }

    /// <summary>
    /// Запускает сборку подземелья для заданного уровня.
    /// </summary>
    /// <param name="currentDungeonLevel">SO-объект уровня, содержащий граф и шаблоны</param>
    /// <returns>true, если сборка прошла успешно</returns>
    public bool GenerateDungeon(DungeonLevelSO currentDungeonLevel)
    {
        dungeonLevel = currentDungeonLevel;
        roomTemplateList = dungeonLevel.roomTemplateList;
        LoadRoomTemplatesIntoDictionary();

        bool built = false;
        int attempts = 0;
        while (!built && attempts < Settings.maxDungeonBuildAttempts)
        {
            attempts++;
            ClearDungeon();

            // Начинаем сборку по графу из SO
            var graph = dungeonLevel.roomNodeGraph;
            built = AttemptToBuildRandomDungeon(graph);

            if (built)
                InstantiateRoomGameobjects();
        }

        return built;
    }

    /// <summary>
    /// Кладёт все шаблоны текущего уровня в словарь по GUID.
    /// </summary>
    private void LoadRoomTemplatesIntoDictionary()
    {
        roomTemplateDictionary.Clear();
        foreach (var template in roomTemplateList)
        {
            if (template != null && !roomTemplateDictionary.ContainsKey(template.guid))
                roomTemplateDictionary[template.guid] = template;
        }
    }

    /// <summary>
    /// Проходит по дереву RoomNodeGraphSO и размещает все комнаты, соединяя их дверями.
    /// </summary>
    private bool AttemptToBuildRandomDungeon(RoomNodeGraphSO roomNodeGraph)
    {
        var openQueue = new Queue<RoomNodeSO>();

        // Ищем входную ноду
        var entranceType = roomNodeTypeList.list.Find(t => t.isEntrance);
        var entranceNode = roomNodeGraph.GetRoomNode(entranceType);
        if (entranceNode == null)
        {
            Debug.LogError("DungeonBuilder: no entrance node found");
            return false;
        }

        openQueue.Enqueue(entranceNode);

        // Размещаем все ноды в очереди
        return ProcessRoomsInOpenRoomNodeQueue(roomNodeGraph, openQueue, true);
    }


    /// <summary>
    /// Обходит очередь узлов, создаёт и позиционирует комнаты,
    /// подбирая стык дверей так, чтобы не было наложений.
    /// </summary>
    private bool ProcessRoomsInOpenRoomNodeQueue(
        RoomNodeGraphSO roomNodeGraph,
        Queue<RoomNodeSO> openRoomNodeQueue,
        bool _ignoreOverlap)   // остаётся для сигнатуры, но не используется
    {
        while (openRoomNodeQueue.Count > 0)
        {
            var node = openRoomNodeQueue.Dequeue();
            Room placedRoom;
            DoorWay parentDoor = null, childDoor = null;

            // 1) Входная (root) комната — просто в (0,0)
            if (node.parentRoomNodeIDList.Count == 0)
            {
                var template = GetRandomRoomTemplate(node.roomNodeType) ?? defaultRoomTemplate;
                placedRoom  = CreateRoomFromRoomTemplate(template, node);
                var offset  = -placedRoom.templateLowerBounds;
                placedRoom.lowerBounds    += offset;
                placedRoom.upperBounds    += offset;
                placedRoom.isPositioned    = true;
            }
            else
            {
                // 2) Дочерняя — находим безопасную стыковку
                var parentId = node.parentRoomNodeIDList[0];
                if (!dungeonBuilderRoomDictionary.TryGetValue(parentId, out var parentRoom))
                {
                    Debug.LogError($"[DungeonBuilder] Parent '{parentId}' not found");
                    return false;
                }

                if (!TryFindRoomPlacement(node, parentRoom,
                        out placedRoom, out parentDoor, out childDoor))
                {
                    Debug.LogError($"[DungeonBuilder] Couldn't place room '{node.id}' without overlap");
                    return false;
                }

                // Соединяем двери и рисуем коридор
                parentDoor.isConnected = true;
                childDoor .isConnected = true;
                dungeonBuilderRoomDictionary[placedRoom.id] = placedRoom;

                AddCorridorBetweenRooms(parentRoom, parentDoor, placedRoom, childDoor);
            }

            // 3) Регистрируем комнату (для проверки следующих)
            dungeonBuilderRoomDictionary[placedRoom.id] = placedRoom;
            Debug.Log($"[DungeonBuilder] Placed '{placedRoom.id}' at {placedRoom.lowerBounds}–{placedRoom.upperBounds}");

            // 4) В очередь — всех детей
            foreach (var child in roomNodeGraph.GetChildRoomNodes(node))
                openRoomNodeQueue.Enqueue(child);
        }

        return true;
    }

    /// <summary>
    /// Пытается найти для node в родительской комнате parent такую комбинацию
    /// (дверь родителя, шаблон, дверь ребёнка), что newBounds не перекрываются
    /// ни с одной уже сохранённой комнатой. Если не найдёт — возвращает первый вариант.
    /// </summary>
    private bool TryFindRoomPlacement(
        RoomNodeSO node,
        Room parentRoom,
        out Room placedRoom,
        out DoorWay outParentDoor,
        out DoorWay outChildDoor)
    {
        placedRoom     = null;
        outParentDoor  = null;
        outChildDoor   = null;

        // Перебираем каждую дверь родителя
        foreach (var pd in parentRoom.doorWayList)
        {
            // Все подходящие шаблоны для этой ноды
            var templates = GetTemplatesForRoomConsistentWithParent(node, pd);
            foreach (var tpl in templates)
            {
                // Создаём "чистый" экземпляр комнаты
                var candidate = CreateRoomFromRoomTemplate(tpl, node);

                // Каждую дверь ребёнка
                foreach (var cd in candidate.doorWayList)
                {
                    // Вычисляем смещение, чтобы стыковать pd ↔ cd
                    var parentWorld = parentRoom.lowerBounds
                                      + pd.position
                                      - parentRoom.templateLowerBounds;
                    var offset      = parentWorld - cd.position;

                    // Границы, куда встанет candidate
                    var lb = candidate.templateLowerBounds + offset;
                    var ub = candidate.templateUpperBounds + offset;

                    // Если без пересечений — сразу принимаем
                    if (!IsOverlappingAnyRoom(lb, ub))
                    {
                        candidate.lowerBounds  = lb;
                        candidate.upperBounds  = ub;
                        candidate.isPositioned = true;

                        placedRoom    = candidate;
                        outParentDoor = pd;
                        outChildDoor  = cd;
                        return true;
                    }
                }
            }
        }

        // Если ничего "чистого" не найдено — первый вариант любой двери и шаблона
        {
            var pd = parentRoom.doorWayList[0];
            var tpl = GetTemplatesForRoomConsistentWithParent(node, pd)
                      .FirstOrDefault() ?? defaultRoomTemplate;
            var candidate = CreateRoomFromRoomTemplate(tpl, node);
            var cd        = candidate.doorWayList[0];

            var parentWorld = parentRoom.lowerBounds
                              + pd.position
                              - parentRoom.templateLowerBounds;
            var offset = parentWorld - cd.position;

            candidate.lowerBounds  = candidate.templateLowerBounds + offset;
            candidate.upperBounds  = candidate.templateUpperBounds + offset;
            candidate.isPositioned = true;

            placedRoom    = candidate;
            outParentDoor = pd;
            outChildDoor  = cd;
            return true;
        }
    }
    
    /// <summary>
    /// Возвращает все шаблоны комнат для данной RoomNodeSO,
    /// отфильтрованные по типу и (для коридоров) по ориентации parentDoor.
    /// </summary>
    private IEnumerable<RoomTemplateSO> GetTemplatesForRoomConsistentWithParent(
        RoomNodeSO node,
        DoorWay parentDoor)
    {
        // Если это коридор, то берём только NS- или EW-шаблоны в зависимости от ориентации двери родителя
        if (node.roomNodeType.isCorridor)
        {
            bool northSouth = parentDoor.orientation == Orientation.north
                              || parentDoor.orientation == Orientation.south;
            return roomTemplateList.Where(tpl =>
                northSouth
                    ? tpl.roomNodeType.isCorridorNS
                    : tpl.roomNodeType.isCorridorEW
            );
        }

        // Иначе — все шаблоны точно такого же типа комнаты
        return roomTemplateList.Where(tpl =>
            tpl.roomNodeType == node.roomNodeType
        );
    }


    /// <summary>
    /// Проверяет, накладывается ли прямоугольник [lower;upper) на любую уже поставленную комнату.
    /// </summary>
    private bool IsOverlappingAnyRoom(Vector2Int lower, Vector2Int upper)
    {
        foreach (var r in dungeonBuilderRoomDictionary.Values)
        {
            if (upper.x  <= r.lowerBounds.x ||
                lower.x  >= r.upperBounds.x ||
                upper.y  <= r.lowerBounds.y ||
                lower.y  >= r.upperBounds.y)
            {
                continue; // не пересекаются
            }
            // оба интервала по X и Y пересекаются → коллизия
            return true;
        }
        return false;
    }


    /// <summary>
    /// Раздвигает одну комнату так, чтобы её прямоугольник [lowerBounds; upperBounds)
    /// не пересекался с остальными, выполняя **один** проход по всем остальным.
    /// </summary>
    private void ResolveOverlapSinglePass(Room room)
    {
        Vector2Int totalShift = Vector2Int.zero;

        var center = (room.lowerBounds + room.upperBounds) / 2;

        foreach (var other in dungeonBuilderRoomDictionary.Values)
        {
            if (other.id == room.id) continue;

            // Проверяем пересечение
            bool overlapX = room.upperBounds.x > other.lowerBounds.x &&
                            room.lowerBounds.x < other.upperBounds.x;
            bool overlapY = room.upperBounds.y > other.lowerBounds.y &&
                            room.lowerBounds.y < other.upperBounds.y;
            if (!overlapX || !overlapY) 
                continue;

            // Размер перекрытия по осям
            int dx = Mathf.Min(room.upperBounds.x, other.upperBounds.x) -
                     Mathf.Max(room.lowerBounds.x, other.lowerBounds.x);
            int dy = Mathf.Min(room.upperBounds.y, other.upperBounds.y) -
                     Mathf.Max(room.lowerBounds.y, other.lowerBounds.y);

            // Определяем направление смещения: отталкиваем от центра other
            int dirX = center.x > (other.lowerBounds.x + other.upperBounds.x) / 2 ? 1 : -1;
            int dirY = center.y > (other.lowerBounds.y + other.upperBounds.y) / 2 ? 1 : -1;

            // Выбираем меньшую величину смещения, чтобы раздвинуть быстрее
            if (dx < dy)
                totalShift.x += dirX * (dx + 1);
            else
                totalShift.y += dirY * (dy + 1);
        }

        // Наносим единственное смещение
        room.lowerBounds += totalShift;
        room.upperBounds += totalShift;
    }
    



    /// <summary>
    /// Создаёт коридор между двумя комнатами, копируя тайлы между дверными позициями.
    /// </summary>
    private void AddCorridorBetweenRooms(Room parent, DoorWay pd, Room child, DoorWay cd)
    {
        var start = parent.lowerBounds + pd.position - parent.templateLowerBounds;
        var end   = child .lowerBounds + cd.position - child .templateLowerBounds;

        var tiles = new List<Vector2Int>();
        if (start.x != end.x)
        {
            int minX = Mathf.Min(start.x, end.x), maxX = Mathf.Max(start.x, end.x);
            for (int x = minX; x <= maxX; x++)
                tiles.Add(new Vector2Int(x, start.y));
        }
        if (start.y != end.y)
        {
            int minY = Mathf.Min(start.y, end.y), maxY = Mathf.Max(start.y, end.y);
            for (int y = minY; y <= maxY; y++)
                tiles.Add(new Vector2Int(end.x, y));
        }

        foreach (var tile in tiles)
            Debug.Log($"[DungeonBuilder] Corridor tile at {tile}");
    }

    /// <summary>
    /// Возвращает шаблон комнаты указанного типа из словаря. Может вернуть null.
    /// </summary>
    public RoomTemplateSO GetRoomTemplate(string guid)
    {
        roomTemplateDictionary.TryGetValue(guid, out var tpl);
        return tpl;
    }

    /// <summary>
    /// Возвращает уже созданную комнату по её ID или null.
    /// </summary>
    public Room GetRoomByRoomID(string roomID)
    {
        dungeonBuilderRoomDictionary.TryGetValue(roomID, out var room);
        return room;
    }

    /// <summary>
    /// Инстанцирует игровые объекты всех комнат из словаря.
    /// </summary>
    private void InstantiateRoomGameobjects()
    {
        foreach (var kv in dungeonBuilderRoomDictionary)
        {
            var room = kv.Value;
            var worldPos = new Vector3(
                room.lowerBounds.x - room.templateLowerBounds.x,
                room.lowerBounds.y - room.templateLowerBounds.y,
                0f);

            var go = Instantiate(room.prefab, worldPos, Quaternion.identity, transform);
            var inst = go.GetComponentInChildren<InstantiatedRoom>();
            inst.room = room;
            inst.Initialise(go);
            room.instantiatedRoom = inst;
        }
    }

    /// <summary>
    /// Удаляет все ранее созданные игровые объекты комнат и очищает словарь.
    /// </summary>
    private void ClearDungeon()
    {
        foreach (var kv in dungeonBuilderRoomDictionary)
        {
            var inst = kv.Value.instantiatedRoom;
            if (inst != null)
                Destroy(inst.gameObject);
        }
        dungeonBuilderRoomDictionary.Clear();
    }

    /// <summary>
    /// Клонирует RoomTemplateSO в новый объект Room, заполняя все поля.
    /// </summary>
    private Room CreateRoomFromRoomTemplate(RoomTemplateSO tpl, RoomNodeSO node)
    {
        if (tpl == null || node == null)
            return null;

        var room = new Room
        {
            id                   = node.id,
            templateID           = tpl.guid,
            prefab               = tpl.prefab,
            roomNodeType         = tpl.roomNodeType,
            lowerBounds          = tpl.lowerBounds,
            upperBounds          = tpl.upperBounds,
            templateLowerBounds  = tpl.lowerBounds,
            templateUpperBounds  = tpl.upperBounds,
            spawnPositionArray   = tpl.spawnPositionArray,
            childRoomIDList      = new List<string>(node.childRoomNodeIDList),
            doorWayList          = tpl.doorwayList.Select(d =>
                new DoorWay
                {
                    position                = d.position,
                    orientation             = d.orientation,
                    doorPrefab              = d.doorPrefab,
                    doorwayStartCopyPosition= d.doorwayStartCopyPosition,
                    doorwayCopyTileWidth    = d.doorwayCopyTileWidth,
                    doorwayCopyTileHeight   = d.doorwayCopyTileHeight
                }).ToList(),
            parentRoomID         = node.parentRoomNodeIDList.FirstOrDefault() ?? "",
            isPreviouslyVisited   = node.parentRoomNodeIDList.Count == 0
        };

        return room;
    }

    /// <summary>
    /// Возвращает случайный шаблон комнаты для заданного типа узла.
    /// </summary>
    private RoomTemplateSO GetRandomRoomTemplate(RoomNodeTypeSO type)
    {
        var list = roomTemplateList.Where(tpl => tpl.roomNodeType == type).ToList();
        return list.Count > 0
            ? list[Random.Range(0, list.Count)]
            : null;
    }

    /// <summary>
    /// Для коридорных нод выбирает NS/EW-шаблон в зависимости от ориентации родительской двери.
    /// </summary>
    private RoomTemplateSO GetRandomTemplateForRoomConsistentWithParent(RoomNodeSO node, DoorWay parentDoor)
    {
        if (node.roomNodeType.isCorridor)
        {
            var type = (parentDoor.orientation == Orientation.north || parentDoor.orientation == Orientation.south)
                ? roomNodeTypeList.list.Find(t => t.isCorridorNS)
                : roomNodeTypeList.list.Find(t => t.isCorridorEW);

            return GetRandomRoomTemplate(type);
        }
        return GetRandomRoomTemplate(node.roomNodeType);
    }

    /// <summary>
    /// Возвращает все двери комнаты, которые ещё не соединены и доступны.
    /// </summary>
    private IEnumerable<DoorWay> GetUnconnectedAvailableDoorways(List<DoorWay> list)
    {
        return list.Where(d => !d.isConnected && !d.isUnavailable);
    }
}

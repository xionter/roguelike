using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ProceduralDungeonGenerator
{
    public enum Difficulty { Easy, Medium, Hard }

    public static void GenerateDungeon(RoomNodeGraphSO graph, int totalRooms, Difficulty difficulty)
    {
        if (graph == null)
        {
            Debug.LogError("DungeonGenerator: graph is null.");
            return;
        }
        if (graph.roomNodeTypeList == null)
            graph.roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
        if (graph.roomNodeTypeList == null || graph.roomNodeTypeList.list == null || graph.roomNodeTypeList.list.Count == 0)
        {
            Debug.LogError("DungeonGenerator: roomNodeTypeList is not set or empty.");
            return;
        }
        graph.roomNodeList.Clear();
        graph.roomNodeDictionary.Clear();

        Random.InitState(System.DateTime.Now.Millisecond);

        // Создаем ноды
        RoomNodeSO CreateNode(RoomNodeTypeSO type, RoomNodeGraphSO graph)
        {
            var node = ScriptableObject.CreateInstance<RoomNodeSO>();
            node.rect = new Rect(Vector2.zero, new Vector2(160, 75));
            node.roomNodeGraph = graph;
            node.roomNodeType = type;

            // Генерация уникального идентификатора для узла
            node.id = System.Guid.NewGuid().ToString();

            graph.roomNodeList.Add(node);
            graph.roomNodeDictionary[node.id] = node;

            return node;
        }

        // Типы комнат
        var types = graph.roomNodeTypeList.list;
        var entranceType = types.Find(t => t.isEntrance);
        var bossType = types.Find(t => t.isBossRoom);
        var corridorType = types.Find(t => t.isCorridor);
        var chestType = types.Find(t => t.roomNodeTypeName.ToLower().Contains("chest"));
        var smallType = types.Find(t => t.roomNodeTypeName.ToLower().Contains("small"));
        var mediumType = types.Find(t => t.roomNodeTypeName.ToLower().Contains("med"));
        var largeType = types.Find(t => t.roomNodeTypeName.ToLower().Contains("large"));

        // Начальная комната
        var startNode = CreateNode(entranceType, graph);
        var openList = new List<RoomNodeSO> { startNode };
        var normalCount = totalRooms - 2;
        var chestCount = Random.Range(1, normalCount / 4);
        normalCount -= chestCount;
        var weights = GetWeights(difficulty);

        // Генерация обычных комнат
        for (var i = 0; i < normalCount; i++)
        {
            var normalParent = openList[Random.Range(0, openList.Count)];
            var cor = CreateNode(corridorType, graph);
            normalParent.childRoomNodeIDList.Add(cor.id);
            cor.parentRoomNodeIDList.Add(normalParent.id);

            var sizeType = SampleRoomType(weights, smallType, mediumType, largeType);
            var newRoom = CreateNode(sizeType, graph);
            cor.childRoomNodeIDList.Add(newRoom.id);
            newRoom.parentRoomNodeIDList.Add(cor.id);

            openList.Add(newRoom);
            if (normalParent.childRoomNodeIDList.Count >= Settings.maxChildCorridors)
                openList.Remove(normalParent);
        }

        // Генерация сундуков
        for (var i = 0; i < chestCount; i++)
        {
            RoomNodeSO chestParent;

            if (i == 0)
            {
                // Первая комната с сундуком создаётся рядом с начальной комнатой
                chestParent = startNode;
            }
            else
            {
                // Остальные сундуки генерируются как обычно
                chestParent = openList[Random.Range(0, openList.Count)];
            }

            var cor = CreateNode(corridorType, graph);
            chestParent.childRoomNodeIDList.Add(cor.id);
            cor.parentRoomNodeIDList.Add(chestParent.id);

            var chest = CreateNode(chestType, graph);
            cor.childRoomNodeIDList.Add(chest.id);
            chest.parentRoomNodeIDList.Add(cor.id);

            if (i != 0) // Удаляем из openList только для остальных сундуков
            {
                openList.Remove(chestParent);
            }
            openList.Add(chest);
        }

        // Генерация комнаты босса
        var candidates = openList.Where(r => GetRoomDepth(r, graph) >= 3).ToList();
        var bossParent = candidates.Count > 0
            ? candidates[Random.Range(0, candidates.Count)]
            : openList[Random.Range(0, openList.Count)];

        var corBoss = CreateNode(corridorType, graph);
        bossParent.childRoomNodeIDList.Add(corBoss.id);
        corBoss.parentRoomNodeIDList.Add(bossParent.id);

        var boss = CreateNode(bossType, graph);
        corBoss.childRoomNodeIDList.Add(boss.id);
        boss.parentRoomNodeIDList.Add(corBoss.id);
    }

    private static Dictionary<string, float> GetWeights(Difficulty difficulty)
    {

        return difficulty switch
        {
            Difficulty.Easy => new Dictionary<string, float> { { "small", 0.5f }, { "medium", 0.3f }, { "large", 0.2f } },
            Difficulty.Medium => new Dictionary<string, float> { { "small", 0.4f }, { "medium", 0.4f }, { "large", 0.2f } },
            Difficulty.Hard => new Dictionary<string, float> { { "small", 0.3f }, { "medium", 0.5f }, { "large", 0.2f } },
            _ => new Dictionary<string, float> { { "small", 0.5f }, { "medium", 0.3f }, { "large", 0.2f } }
        };
    }

    private static RoomNodeTypeSO SampleRoomType(Dictionary<string, float> weights, RoomNodeTypeSO smallType, RoomNodeTypeSO mediumType, RoomNodeTypeSO largeType)
    {
        var randomValue = Random.value;
        if (randomValue < weights["small"]) return smallType;
        if (randomValue < weights["small"] + weights["medium"]) return mediumType;
        return largeType;
    }

    private static int GetRoomDepth(RoomNodeSO roomNode, RoomNodeGraphSO graph)
    {
        var depth = 0;
        var currentNode = roomNode;
        while (currentNode.parentRoomNodeIDList.Count > 0)
        {
            depth++;
            currentNode = graph.roomNodeDictionary[currentNode.parentRoomNodeIDList[0]];
        }
        return depth;
    }
}
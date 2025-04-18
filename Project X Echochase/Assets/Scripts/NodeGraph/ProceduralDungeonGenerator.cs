using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

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
#if UNITY_EDITOR


        AssetDatabase.SaveAssets();
        Random.InitState(System.DateTime.Now.Millisecond);
        
        // создаем ноды йоу
        RoomNodeSO CreateNode(RoomNodeTypeSO type)
        {
            var node = ScriptableObject.CreateInstance<RoomNodeSO>();
            node.Initialise(new Rect(Vector2.zero, new Vector2(160,75)), graph, type);
            graph.roomNodeList.Add(node);
            graph.roomNodeDictionary[node.id] = node;
            AssetDatabase.AddObjectToAsset(node, graph);
            return node;
        }

        // Типы
        var types         = graph.roomNodeTypeList.list;
        var entranceType  = types.Find(t => t.isEntrance);
        var bossType      = types.Find(t => t.isBossRoom);
        var corridorType  = types.Find(t => t.isCorridor);
        var chestType     = types.Find(t => t.roomNodeTypeName.ToLower().Contains("chest"));
        var smallType     = types.Find(t => t.roomNodeTypeName.ToLower().Contains("small"));
        var mediumType    = types.Find(t => t.roomNodeTypeName.ToLower().Contains("med"));
        var largeType     = types.Find(t => t.roomNodeTypeName.ToLower().Contains("large"));

        // Начало
        var startNode = CreateNode(entranceType);
        var openList  = new List<RoomNodeSO> { startNode };
        var normalCount = totalRooms - 2;
        var chestCount  = Random.Range((int)(GetWeights(difficulty)["large"] / 0.2f) + 1, normalCount / 4);
        normalCount -= chestCount;
        var weights     = GetWeights(difficulty);

        // Генерация обычных
        for (var i = 0; i < normalCount; i++)
        {
            var parent = openList[Random.Range(0, openList.Count)];
            var cor    = CreateNode(corridorType);
            parent.childRoomNodeIDList.Add(cor.id);
            cor.parentRoomNodeIDList.Add(parent.id);

            var sizeType = SampleRoomType(weights, smallType, mediumType, largeType);
            var newRoom  = CreateNode(sizeType);
            cor.childRoomNodeIDList.Add(newRoom.id);
            newRoom.parentRoomNodeIDList.Add(cor.id);

            openList.Add(newRoom);
            if (parent.childRoomNodeIDList.Count >= Settings.maxChildCorridors)
                openList.Remove(parent);
        }

        // Сундуки
        {
            for (var i = 0; i < chestCount; i++)
            {
                var parent = openList[Random.Range(0, openList.Count)];
                var cor = CreateNode(corridorType);
                parent.childRoomNodeIDList.Add(cor.id);
                cor.parentRoomNodeIDList.Add(parent.id);

                var chest = CreateNode(chestType);
                cor.childRoomNodeIDList.Add(chest.id);
                chest.parentRoomNodeIDList.Add(cor.id);

                openList.Remove(parent);
                openList.Add(chest);
            }
        }

        // Босс
        {
            var candidates = openList.Where(r => GetRoomDepth(r, graph) >= 3).ToList();
            var parent = candidates.Count > 0
                ? candidates[Random.Range(0, candidates.Count)]
                : openList[Random.Range(0, openList.Count)];

            var cor   = CreateNode(corridorType);
            parent.childRoomNodeIDList.Add(cor.id);
            cor.parentRoomNodeIDList.Add(parent.id);

            var boss  = CreateNode(bossType);
            cor.childRoomNodeIDList.Add(boss.id);
            boss.parentRoomNodeIDList.Add(cor.id);
        }


        LayoutGraph(graph);

        EditorUtility.SetDirty(graph);
        AssetDatabase.SaveAssets();
        graph.OnValidate();
#endif
    }

#if UNITY_EDITOR

    private static void LayoutGraph(RoomNodeGraphSO graph)
    {

        var start = graph.roomNodeList.Find(n => n.roomNodeType.isEntrance);
        if (start == null) return;
        var queue = new Queue<RoomNodeSO>();
        var depth = new Dictionary<string, int>();
        queue.Enqueue(start);
        depth[start.id] = 0;
        while (queue.Count > 0)
        {
            var node = queue.Dequeue();
            var d   = depth[node.id];
            foreach (var childID in node.childRoomNodeIDList)
            {
                if (!depth.ContainsKey(childID))
                {
                    depth[childID] = d + 1;
                    queue.Enqueue(graph.GetRoomNode(childID));
                }
            }
        }


        var groups = depth.GroupBy(kv => kv.Value).OrderBy(g => g.Key);
        float xSpacing = 200f, ySpacing = 150f;
        foreach (var group in groups)
        {
            var count = group.Count();
            var idx   = 0;
            foreach (var kv in group)
            {
                var node = graph.GetRoomNode(kv.Key);
                var x  = idx * xSpacing - (count - 1) * xSpacing / 2;
                var y  = kv.Value * ySpacing;
                node.rect.position = new Vector2(x, y);
                EditorUtility.SetDirty(node);
                idx++;
            }
        }
    }
#endif

    private static Dictionary<string, float> GetWeights(Difficulty d)
    {
        switch (d)
        {
            case Difficulty.Easy:   return new Dictionary<string, float> {{"small",0.6f},{"med",0.3f},{"large",0.1f}};
            case Difficulty.Medium: return new Dictionary<string, float> {{"small",0.3f},{"med",0.5f},{"large",0.2f}};
            case Difficulty.Hard:   return new Dictionary<string, float> {{"small",0.1f},{"med",0.4f},{"large",0.6f}};
            default:                return new Dictionary<string, float> {{"small",0.3f},{"med",0.4f},{"large",0.3f}};
        }
    }

    private static RoomNodeTypeSO SampleRoomType(
        Dictionary<string, float> weights,
        RoomNodeTypeSO smallType,
        RoomNodeTypeSO medType,
        RoomNodeTypeSO largeType)
    {
        var total = weights.Values.Sum();
        var r     = Random.value * total;
        var acc   = 0f;
        foreach (var kv in weights)
        {
            acc += kv.Value;
            if (r <= acc)
            {
                switch (kv.Key)
                {
                    case "small": return smallType;
                    case "med":   return medType;
                    case "large": return largeType;
                }
            }
        }
        return medType;
    }


    private static int GetRoomDepth(RoomNodeSO room, RoomNodeGraphSO graph)
    {
        int depth = 0;
        var current = room;
        while (current.parentRoomNodeIDList.Count > 0)
        {
            var p = graph.GetRoomNode(current.parentRoomNodeIDList[0]);
            if (p.roomNodeType.isCorridor && p.parentRoomNodeIDList.Count > 0)
                p = graph.GetRoomNode(p.parentRoomNodeIDList[0]);
            depth++;
            current = p;
        }
        return depth;
    }
}

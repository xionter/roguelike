using UnityEditor;
using UnityEngine;

public class GenerateDungeonWindow : EditorWindow
{
    private RoomNodeGraphSO graph;
    private int totalRooms = 10;
    private ProceduralDungeonGenerator.Difficulty difficulty = ProceduralDungeonGenerator.Difficulty.Medium;

    // Открыть окно и передать в него граф
    public static void Open(RoomNodeGraphSO graph)
    {
        var window = GetWindow<GenerateDungeonWindow>("Generate Dungeon");
        window.graph = graph;
        window.minSize = new Vector2(250, 120);
    }

    private void OnGUI()
    {
        GUILayout.Label("Dungeon Settings", EditorStyles.boldLabel);
        EditorGUI.BeginChangeCheck();
        
        totalRooms = EditorGUILayout.IntField("Total Rooms", totalRooms);
        difficulty = (ProceduralDungeonGenerator.Difficulty)
            EditorGUILayout.EnumPopup("Difficulty", difficulty);

        if (EditorGUI.EndChangeCheck())
        {
            totalRooms = Mathf.Max(3, totalRooms); // минимум 3 комнаты
        }

        GUILayout.FlexibleSpace();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate", GUILayout.Height(30)))
        {
            // Запускаем генерацию и закрываем окно
            ProceduralDungeonGenerator.GenerateDungeon(graph, totalRooms, difficulty);
            
            Close();
        }
        if (GUILayout.Button("Cancel", GUILayout.Height(30)))
        {
            Close();
        }
        GUILayout.EndHorizontal();
    }
}
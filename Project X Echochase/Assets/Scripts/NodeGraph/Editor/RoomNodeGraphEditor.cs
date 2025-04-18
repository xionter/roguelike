using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

public class RoomNodeGraphEditor : EditorWindow
{
    private GUIStyle roomNodeStyle;
    private GUIStyle roomNodeSelectedStyle;
    
    private static RoomNodeGraphSO currentRoomNodeGraph;
    private RoomNodeSO currentRoomNode = null;
    private RoomNodeTypeListSO roomNodeTypeList;

    private Vector2 graphOffset;
    private Vector2 graphDrag;
    
    private const float nodeWidth = 160f;
    private const float nodeHeight = 75f;
    private const int nodePadding = 25;
    private const int nodeBorder = 12;
    
    private const float connectingLineWidth = 3f;
    private const float connectingLineArrowSize = 6f;

    private const float gridLarge = 100f;
    private const float gridSmall = 25f;
    
    [MenuItem("Room Node Graph Editor", menuItem = "Window/Dungeon Editor/Room Node Graph Editor")]

    private static void OpenWindow()
    {
        GetWindow<RoomNodeGraphEditor>("Room Node Graph Editor"); 
    }
    
    private void OnEnable()
    {
        Selection.selectionChanged += InspectorSelectionChanged;
        
        roomNodeStyle = new GUIStyle();
        roomNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
        roomNodeStyle.normal.textColor = Color.white;
        roomNodeStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        roomNodeStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);
        
        roomNodeSelectedStyle = new GUIStyle();
        roomNodeSelectedStyle.normal.background = EditorGUIUtility.Load("node1 on") as Texture2D;
        roomNodeSelectedStyle.normal.textColor = Color.white;
        roomNodeSelectedStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        roomNodeSelectedStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);
        
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }
    
    
    
    [OnOpenAsset(0)]
    public static bool OnDoubleClickAsset(int instanceID, int line)
    {
        var roomNodeGraph = EditorUtility.InstanceIDToObject(instanceID) as RoomNodeGraphSO; 
        if (roomNodeGraph != null)
        {
            OpenWindow();
            currentRoomNodeGraph = roomNodeGraph;
            return true;
        }

        return false;
    }
    
    
    
    

    private void OnGUI()
    {
        if (currentRoomNodeGraph != null)
        {
            DrawBackgroundGrid(gridSmall, 0.2f, Color.gray);
            DrawBackgroundGrid(gridLarge, 0.3f, Color.gray);
            
            DrawDraggedLine();
            
            ProcessEvents(Event.current);

            DrawRoomNodeConnections();
            
            DrawRoomNodes();
        }
        
        if (GUI.changed)
        {
            Repaint();
        }
    }
    
    private void DrawBackgroundGrid(float gridSize, float gridOpacity, Color gridColor)
    {
        var verticalLineCount = Mathf.CeilToInt((position.width + gridSize) / gridSize);
        var horizontalLineCount = Mathf.CeilToInt((position.height + gridSize) / gridSize);
        
        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity); 
        
        var gridOffset = new Vector3(graphOffset.x % gridSize, graphOffset.y % gridSize, 0);

        for (var i = 0; i < verticalLineCount; i++)
        {
            Handles.DrawLine(new Vector3(gridSize * i, -gridSize, 0) + gridOffset, new Vector3(gridSize * i, position.height + gridSize, 0f) + gridOffset);
        }
        for (var j = 0; j < horizontalLineCount; j++)
        {
            Handles.DrawLine(new Vector3(-gridSize, gridSize * j, 0) + gridOffset, new Vector3(position.width + gridSize, gridSize * j, 0f) + gridOffset);
        }

        Handles.color = Color.white;
        
        
        graphOffset += graphDrag * 0.5f;
        
    }
    
    private void DrawDraggedLine()
    {
        if (currentRoomNodeGraph.linePosition != Vector2.zero)
        {
            Handles.DrawBezier(currentRoomNodeGraph.roomNodeToDrawLineFrom.rect.center, currentRoomNodeGraph.linePosition,
                currentRoomNodeGraph.roomNodeToDrawLineFrom.rect.center, currentRoomNodeGraph.linePosition,
                Color.white, null, connectingLineWidth);
        }
    }

    private void ProcessEvents(Event currentEvent)
    {
        graphDrag = Vector2.zero;
        
        if (currentRoomNode == null || currentRoomNode.isLeftClickDragging == false)
            currentRoomNode = IsMouseOverRoomNode(currentEvent);
        if(currentRoomNode == null || currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
            ProcessRoomNodeGraphEvents(currentEvent);
        else
            currentRoomNode.ProcessEvents(currentEvent);
    }
    
    private RoomNodeSO IsMouseOverRoomNode(Event currentEvent)
    {
        return currentRoomNodeGraph.roomNodeList.ToArray().Reverse().FirstOrDefault(roomNode => roomNode.rect.Contains(currentEvent.mousePosition));
    }
    
    private void ProcessRoomNodeGraphEvents(Event currentEvent)
    {
        switch (currentEvent.type)
        {
            case EventType.MouseDown:
                ProcessMouseDownEvent(currentEvent);
                break;
            case EventType.MouseUp:
                ProcessMouseUpEvent(currentEvent);
                break;
            case EventType.MouseDrag:
                ProcessMouseDragEvent(currentEvent);
                break;
            default:
                break;
        }
    }
    
    private void ProcessMouseDownEvent(Event currentEvent)
    {
        if (currentEvent.button == 1) //пкм
        {
            ShowContextMenu(currentEvent.mousePosition);
        }
        else if (currentEvent.button == 0) //лкм
        {
            ClearLineDrag();
            ClearAllSelectedRoomNodes();
        }
    }

    private void ShowContextMenu(Vector2 mousePosition)
    {
        var menu = new GenericMenu();
        menu.AddItem(new GUIContent("Create Room Node"), false, CreateRoomNode, mousePosition);
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("Select All"), false, SelectAllRoomNodes);
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("Delete Selected Room Node Links"), false, DeleteSelectedRoomNodeLinks);
        menu.AddItem(new GUIContent("Delete Selected Room Nodes"), false, DeleteSelectedRoomNodes);
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("Generate Dungeon"), false, GenerateDungeon);
        menu.ShowAsContext();
        
    }

    private void GenerateDungeon()
    {
        if (currentRoomNodeGraph != null)
        {
            GenerateDungeonWindow.Open(currentRoomNodeGraph);
        }
    }
    private void CreateRoomNode(object mousePositionObject )
    {
        if (currentRoomNodeGraph.roomNodeList.Count == 0)
        {
            CreateRoomNode(new Vector2(200f, 200f), roomNodeTypeList.list.Find(x => x.isEntrance));
        }
        
        CreateRoomNode(mousePositionObject, roomNodeTypeList.list.Find(x => x.isNone));
    }
    
    private void CreateRoomNode(object mousePositionObject, RoomNodeTypeSO roomNodeType)
    {
        var mousePosition = (Vector2)mousePositionObject;
        var roomNode = ScriptableObject.CreateInstance<RoomNodeSO>();
        currentRoomNodeGraph.roomNodeList.Add(roomNode);
        roomNode.Initialise(new Rect(mousePosition, new Vector2(nodeWidth, nodeHeight)), currentRoomNodeGraph, roomNodeType);
        AssetDatabase.AddObjectToAsset(roomNode, currentRoomNodeGraph);
        AssetDatabase.SaveAssets();
        
        currentRoomNodeGraph.OnValidate();
    }

    private void DeleteSelectedRoomNodes()
    {
        var roomNodeDeletionQueue = new Queue<RoomNodeSO>();
        
        foreach (var roomNode in currentRoomNodeGraph.roomNodeList)
        {
            if (roomNode.isSelected && !roomNode.roomNodeType.isEntrance)
            {
                roomNodeDeletionQueue.Enqueue(roomNode);
                foreach (var childRoomNodeID in roomNode.childRoomNodeIDList)
                {
                    var childRoomNode = currentRoomNodeGraph.GetRoomNode(childRoomNodeID);
                    if (childRoomNode != null)
                    {
                        childRoomNode.RemoveParentRoomNodeIDFromRoomNode(roomNode.id);
                    }
                }

                foreach (var parentRoomNodeID in roomNode.parentRoomNodeIDList)
                {
                    var parentRoomNode = currentRoomNodeGraph.GetRoomNode(parentRoomNodeID);
                    if (parentRoomNode != null)
                    {
                        parentRoomNode.RemoveChildRoomNodeIDFromRoomNode(roomNode.id);
                    }
                }
            }
        }
        while (roomNodeDeletionQueue.Count > 0)
        {
            var roomNodeToDelete = roomNodeDeletionQueue.Dequeue();
            currentRoomNodeGraph.roomNodeDictionary.Remove(roomNodeToDelete.id);
            currentRoomNodeGraph.roomNodeList.Remove(roomNodeToDelete);
            DestroyImmediate(roomNodeToDelete, true); 
            
            AssetDatabase.SaveAssets();
        }
    }
    
    private void DeleteSelectedRoomNodeLinks()
    {
        foreach (var roomNode in currentRoomNodeGraph.roomNodeList)
        {
            if (roomNode.isSelected && roomNode.childRoomNodeIDList.Count > 0)
            {
                for (var i = roomNode.childRoomNodeIDList.Count - 1; i >= 0; i--)
                {
                    var childRoomNode = currentRoomNodeGraph.GetRoomNode(roomNode.childRoomNodeIDList[i]);
                    if (childRoomNode.isSelected && !childRoomNode.roomNodeType.isEntrance)
                    {
                        roomNode.RemoveChildRoomNodeIDFromRoomNode(childRoomNode.id);
                        childRoomNode.RemoveParentRoomNodeIDFromRoomNode(roomNode.id);
                    }
                    
                }
            }
        }
        ClearAllSelectedRoomNodes();
    }
    
    private void ClearAllSelectedRoomNodes()
    {
        foreach (var roomNode in currentRoomNodeGraph.roomNodeList)
        {
            if (roomNode.isSelected)
            {
                roomNode.isSelected = false;
                GUI.changed = true;
            }
        }
    }
    
    private void SelectAllRoomNodes()
    {
        foreach (var roomNode in currentRoomNodeGraph.roomNodeList)
        {
            roomNode.isSelected = true;
        }
        GUI.changed = true;
    }
    
    private void ProcessMouseUpEvent(Event currentEvent)
    {
        if (currentEvent.button == 1 && currentRoomNodeGraph.roomNodeToDrawLineFrom != null) //пкм
        {
            var roomNode = IsMouseOverRoomNode(currentEvent);
            if (roomNode != null)
            {
                if (currentRoomNodeGraph.roomNodeToDrawLineFrom.AddChildRoomNodeIDToRoomNode(roomNode.id))
                {
                    roomNode.AddParentRoomNodeIDToRoomNode(currentRoomNodeGraph.roomNodeToDrawLineFrom.id);
                }
            }
            
            ClearLineDrag();
        }
    }
    
     
    
    private void ProcessMouseDragEvent(Event currentEvent)
    {
        if (currentEvent.button == 1)
        {
            ProcessRightMouseDragEvent(currentEvent);
        }
        else if (currentEvent.button == 0)
        {
            ProcessLeftMouseDragEvent(currentEvent.delta);
        }
    }
    
    private void ProcessRightMouseDragEvent(Event currentEvent)
    {
        if (currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
        {
            DragConnectingLine(currentEvent.delta);
            GUI.changed = true;
        }
    }
    
    private void ProcessLeftMouseDragEvent(Vector2 delta)
    {
        graphDrag = delta;
        foreach (var roomNode in currentRoomNodeGraph.roomNodeList)
        {
            roomNode.DragNode(delta);
        }
        
        GUI.changed = true;
    }
    
    private void DragConnectingLine(Vector2 delta)
    {
        currentRoomNodeGraph.linePosition += delta;
    }
    
    private void ClearLineDrag()
    {
        currentRoomNodeGraph.linePosition = Vector2.zero;
        currentRoomNodeGraph.roomNodeToDrawLineFrom = null;
        GUI.changed = true;
    }
    
    private void DrawRoomNodeConnections()
    {
        foreach (var roomNode in currentRoomNodeGraph.roomNodeList)
        {
            if (roomNode.childRoomNodeIDList.Count > 0)
            {
                foreach (var childRoomNodeID in roomNode.childRoomNodeIDList)
                {
                    if (currentRoomNodeGraph.roomNodeDictionary.ContainsKey(childRoomNodeID))
                    {
                        DrawConnectionLine(roomNode, currentRoomNodeGraph.roomNodeDictionary[childRoomNodeID]);
                        
                        GUI.changed = true;
                    }
                }
            }
        }
    }
    
    private void DrawConnectionLine(RoomNodeSO parentRoomNode, RoomNodeSO childRoomNode)
    {
        var startPosition = parentRoomNode.rect.center;
        var endPosition = childRoomNode.rect.center;
        var midPosition = (startPosition + endPosition) / 2f;
        var direction = (endPosition - startPosition).normalized;
        
        var arrowTailPoint1 = midPosition - new Vector2(-direction.y, direction.x) * connectingLineArrowSize;
        var arrowTailPoint2 = midPosition + new Vector2(-direction.y, direction.x) * connectingLineArrowSize;
        
        var arrowHeadPoint = midPosition + direction * connectingLineArrowSize;
        
        Handles.DrawBezier(arrowHeadPoint, arrowTailPoint1, arrowHeadPoint, arrowTailPoint1, Color.white, null, connectingLineWidth);
        Handles.DrawBezier(arrowHeadPoint, arrowTailPoint2, arrowHeadPoint, arrowTailPoint2, Color.white, null, connectingLineWidth);
        
        Handles.DrawBezier(startPosition, endPosition, startPosition, endPosition, Color.white, null, connectingLineWidth);

        GUI.changed = true;
    }
      
    
    private void DrawRoomNodes()
    {
        foreach (var roomNode in currentRoomNodeGraph.roomNodeList)
        {
            if (roomNode.isSelected)
            {
                roomNode.Draw(roomNodeSelectedStyle);
            }
            else
            {
                roomNode.Draw(roomNodeStyle);
            }
        }
        GUI.changed = true; 
    }
    
    private void InspectorSelectionChanged()
    {
        var roomNodeGraph = Selection.activeObject as RoomNodeGraphSO;
        if (roomNodeGraph != null)
        {
            currentRoomNodeGraph = roomNodeGraph;
            GUI.changed = true;
        }
    }
}
 
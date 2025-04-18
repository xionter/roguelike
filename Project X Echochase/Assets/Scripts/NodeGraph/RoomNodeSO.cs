using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

public class RoomNodeSO : ScriptableObject
{
    [HideInInspector] public string id;
    [HideInInspector] public List<string> parentRoomNodeIDList = new List<string>();
    [HideInInspector] public List<string> childRoomNodeIDList = new List<string>();
    [HideInInspector] public RoomNodeGraphSO roomNodeGraph;
    public RoomNodeTypeSO roomNodeType;
    [HideInInspector] public RoomNodeTypeListSO roomNodeTypeList;
    
    #region Editor Code
#if UNITY_EDITOR
    
    [HideInInspector] public Rect rect;
    [HideInInspector] public bool isLeftClickDragging = false;
    [HideInInspector] public bool isSelected = false; 
    
    
    public void Initialise(Rect rect, RoomNodeGraphSO nodeGraph, RoomNodeTypeSO roomNodeType)
    {
        this.rect = rect;
        id = Guid.NewGuid().ToString();
        name = "RoomNode";
        roomNodeGraph = nodeGraph;
        this.roomNodeType = roomNodeType;
        
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }

    public void Draw(GUIStyle nodeStyle)
    {
        GUILayout.BeginArea(rect, nodeStyle);
        EditorGUI.BeginChangeCheck();

        if (parentRoomNodeIDList.Count > 0 || roomNodeType.isEntrance)
        {
            EditorGUILayout.LabelField(roomNodeType.roomNodeTypeName);
        }
        else
        {
            var selected = roomNodeTypeList.list.FindIndex(x => x == roomNodeType);
            var selection = EditorGUILayout.Popup("", selected, GetRoomNodeTypesToDisplay());
        
            roomNodeType = roomNodeTypeList.list[selection];
        }
        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(this);
        GUILayout.EndArea();
    }

    public string[] GetRoomNodeTypesToDisplay()
    {
        var roomArray = new string[roomNodeTypeList.list.Count];
        for (var i = 0; i < roomNodeTypeList.list.Count; i++)
        {
            if (roomNodeTypeList.list[i].displayInNodeGraphEditor)
            {
                roomArray[i] = roomNodeTypeList.list[i].roomNodeTypeName;
            }
        }
        return roomArray;
    }


    public void ProcessEvents(Event currentEvent)
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
            
            default: break;
        }
    }
    
    
    private void ProcessMouseDownEvent(Event currentEvent)
    {
        if (currentEvent.button == 0) //лкм
        {
            ProcessLeftClickDownEvent(currentEvent);
        }
        else if (currentEvent.button == 1) //пкм
        {
            ProcessRightClickDownEvent(currentEvent);
        }
    }
    
    
    private void ProcessRightClickDownEvent(Event currentEvent)
    {
        roomNodeGraph.SetNodeToDrawConnectionLineFrom(this, currentEvent.mousePosition);
    }
    
    private void ProcessLeftClickDownEvent(Event currentEvent)
    {
        Selection.activeObject = this;
        isSelected = !isSelected;
    }
    
    private void ProcessMouseUpEvent(Event currentEvent)
    {
        if (currentEvent.button == 0) //лкм
        {
            ProcessLeftClickUpEvent();
        }
    }
    
    private void ProcessLeftClickUpEvent()
    {
        isLeftClickDragging = false;
    }
    
    private void ProcessMouseDragEvent(Event currentEvent)
    {
        if (currentEvent.button == 0) //лкм
        {
            ProcessLeftClickDragEvent(currentEvent);
        }
    } 
    
    private void ProcessLeftClickDragEvent(Event currentEvent)
    {
        isLeftClickDragging = true;
        DragNode(currentEvent.delta);
        GUI.changed = true;
    }
    
    public void DragNode(Vector2 delta)
    {
        rect.position += delta;
        EditorUtility.SetDirty(this);
    }

    public bool AddChildRoomNodeIDToRoomNode(string childID)
    {
        if (IsChildRoomValid(childID))
        {
            childRoomNodeIDList.Add(childID);
            return true;
        }

        return false;
    }
    
    public bool IsChildRoomValid(string childID)
    {
        var isConnectedBossNodeAlready = false;
        foreach (var roomNode in roomNodeGraph.roomNodeList)
        {
            if (roomNode.roomNodeType.isBossRoom && roomNode.parentRoomNodeIDList.Count > 0)
            {
                isConnectedBossNodeAlready = true;
            }
        }

        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isBossRoom && isConnectedBossNodeAlready)
            return false;
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isNone)
            return false;
        if (childRoomNodeIDList.Contains(childID))
            return false;
        if (id == childID)
            return false;
        if (parentRoomNodeIDList.Contains(childID))
            return false;
        if (roomNodeGraph.GetRoomNode(childID).parentRoomNodeIDList.Count() > 0)
            return false;
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && roomNodeType.isCorridor)
            return false;
        if (!roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && !roomNodeType.isCorridor)
            return false;
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor &&
            childRoomNodeIDList.Count >= Settings.maxChildCorridors)
            return false;
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isEntrance)
            return false; 
        if (!roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && childRoomNodeIDList.Count > 0)
            return false;
        
        return true;
    }
    
    public bool AddParentRoomNodeIDToRoomNode(string parentID)
    {
        parentRoomNodeIDList.Add(parentID);
        return true;
    }
    
    public bool RemoveChildRoomNodeIDFromRoomNode(string childID)
    {
        return childRoomNodeIDList.Remove(childID);
    }
    
    public bool RemoveParentRoomNodeIDFromRoomNode(string parentID)
    {
        return parentRoomNodeIDList.Remove(parentID);
    }
    
#endif

    #endregion
}

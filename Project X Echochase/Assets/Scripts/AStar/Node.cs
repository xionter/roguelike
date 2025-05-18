using System;
using UnityEngine;

public class Node : IComparable<Node>
{
    public Vector2Int gridPosition;
    public int gCost; // расстояние от стартовой точки
    public int hCost; // расстояние до финиша
    public Node parentNode;
    public int FCost => gCost + hCost;
    
    public Node(Vector2Int gridPosition)
    {
        this.gridPosition = gridPosition;
        this.gCost = gCost;
        this.hCost = hCost;
        parentNode = null;
    }
    
    public int CompareTo(Node nodeToCompare)
    {
        var compare = FCost.CompareTo(nodeToCompare.FCost);
        
        if (compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        
        return compare;
    }
}

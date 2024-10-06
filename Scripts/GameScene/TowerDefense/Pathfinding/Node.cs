using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node>
{
    public NodeType NodeType { get; private set; }
    public bool isPoint;
    public bool isStartingPoint;
    public bool isPath;
    public bool isWalkedOn;
    public bool walkable;
    
    public Vector3 worldPosition;
    public int gridX;
    public int gridY;

    public int gCost;
    public int hCost;
    public Node parent;
    int heapIndex;

    public WFCType wfcType;
    public List<WFCType> possibleTypes;
    public bool collapsed;


    public Node(Node other)
    {
        this.gridX = other.gridX;
        this.gridY = other.gridY;
        this.worldPosition = other.worldPosition;
        this.walkable = other.walkable;
        this.NodeType = other.NodeType;
        this.isPoint = other.isPoint;
    }

    public Node(NodeType nt, bool _walkable, Vector3 _worldPos, int _gridX, int _gridY, bool isStartingPoint)
    {
        NodeType = nt;
        walkable = _walkable;
        worldPosition = _worldPos;
        isPoint = false;
        gridX = _gridX;
        gridY = _gridY;
        this.isStartingPoint = isStartingPoint;
        isPath = false;
        isWalkedOn = false;
        wfcType = WFCType.none;
        possibleTypes = new List<WFCType>{ WFCType.none, WFCType.rock, WFCType.tree, WFCType.path, WFCType.fence }; 
        collapsed = false;
   
    }
    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }

    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }
        set
        {
            heapIndex = value;
        }
    }

    public int CompareTo(Node nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return -compare;
    }
}
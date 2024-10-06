using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pathfinding : MonoBehaviour
{

    public Transform seeker, target;

    public Grid grid;

    private List<Node> finalPath;

    private void Awake()
    {
        
    }


    public List<Node> FindPath(Node startNode, Node targetNode, bool pathExist)
    {
        finalPath = new List<Node>();
        Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet.RemoveFirst();
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                RetracePath(startNode, targetNode, pathExist);
                return finalPath;
            }

            foreach (Node neighbour in grid.GetNeighbours(currentNode))
            {
                if (pathExist)
                {
                    if (!neighbour.walkable || closedSet.Contains(neighbour) || !neighbour.isPath)
                    {
                        continue;
                    }
                }
                else
                {
                    if (!neighbour.walkable || closedSet.Contains(neighbour))
                    {
                        continue;
                    }
                }


                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                    else
                    {
                        openSet.UpdateItem(neighbour);
                    }
                }
            }
        }
        return finalPath;
    }

    void RetracePath(Node startNode, Node endNode, bool pathExist)
    {
        //print("Retracted");
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            if (pathExist)
            {
                currentNode.isWalkedOn = true;
                //currentNode.wfcNode.type = WFCType.path;
            }

            currentNode.isPath = true;
            currentNode = currentNode.parent;
        }
        path.Reverse();

        finalPath = path;

    }

    public int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }


}
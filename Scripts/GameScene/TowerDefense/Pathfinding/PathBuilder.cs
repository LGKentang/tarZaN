using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Scripting;

public class PathBuilder : MonoBehaviour
{
    public Pathfinding pf;
    public Grid grid;

    public List<Node> firstPaths, secondPaths, thirdPaths, fourthPaths;

    public List<Node> visitedNodes;

    private void Awake()
    {
        
        grid  = GetComponent<Grid>();
        //pf.grid = grid;
    }

    private void Start()
    {
        visitedNodes = new List<Node>();    
        pf = gameObject.AddComponent<Pathfinding>();

        print(grid == null);

        print(grid.pathNode == null);

        //firstPaths = GeneratePath(grid.GetSpecifiedNodePath(NodeType.first), grid.GetSpecifiedStartingPoint(NodeType.first));
        //secondPaths = GeneratePath(grid.GetSpecifiedNodePath(NodeType.second), grid.GetSpecifiedStartingPoint(NodeType.second));
        //thirdPaths = GeneratePath(grid.GetSpecifiedNodePath(NodeType.third), grid.GetSpecifiedStartingPoint(NodeType.third));
        //fourthPaths = GeneratePath(grid.GetSpecifiedNodePath(NodeType.fourth), grid.GetSpecifiedStartingPoint(NodeType.fourth));


        //pf.FindPath(grid.GetSpecifiedStartingPoint(NodeType.first), grid.targetNode,true);
        //pf.FindPath(grid.GetSpecifiedStartingPoint(NodeType.second), grid.targetNode, true);
        //pf.FindPath(grid.GetSpecifiedStartingPoint(NodeType.third), grid.targetNode, true);
        //pf.FindPath(grid.GetSpecifiedStartingPoint(NodeType.fourth), grid.targetNode, true);

        //print(firstPaths.Count);
        //foreach (Node node in firstPaths)
        //{
        //    print(node.worldPosition);
        //}




    }



    private List<Node> GeneratePath(List<Node> nodes, Node startingPoint)
    {
        List<Node> path = new List<Node>(); 

        Node current = startingPoint;
        Node nextNode = null;

        do
        {
            nextNode = GetNearestNeighbour(nodes, current);
            if (nextNode == null) break;

            nodes.Remove(current);  // Removing the current node from the nodes list
            nodes.Remove(nextNode);  // Removing the nextNode from the nodes list
            path.AddRange(pf.FindPath(current, nextNode, false));  // Adding the path from current to nextNode

            current = nextNode;
        }
        while (nextNode.NodeType != NodeType.target);

        path.AddRange(pf.FindPath(current, grid.targetNode, false));  // Adding the final path


        //do
        //{
        //    nextNode = GetNearestNeighbour(nodes, current);
        //    if (nextNode == null) break;

        //    nodes.Remove(current);
        //    path.AddRange(pf.FindPath(current, nextNode));

        //    current = nextNode;
        //}
        //while (nextNode.NodeType != NodeType.target);

        //path.AddRange(pf.FindPath(current, grid.targetNode));

        return path;
    }

    private Node GetNearestNeighbour(List<Node> nodes, Node startingPoint)
    {
        Node nearestNode = null;
        int nearestDistance = int.MaxValue;

        foreach (Node node in nodes)
        {
            int distance = pf.GetDistance(node, startingPoint);

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestNode = node;
            }
        }

        return nearestNode;
    }



}

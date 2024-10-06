using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using System;
using System.Linq;
using UnityEditor;
using Unity.VisualScripting.Antlr3.Runtime;
//using UnityEditor.Experimental.GraphView;
using Unity.VisualScripting;
using Unity.Mathematics;

public class Grid : MonoBehaviour
{
    public bool onlyDisplayPathGizmos;
    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    public Pathfinding pf;
    Node[,] grid;

    public List<Node> pathNode;
    public List<Node> startList;
    public Node targetNode;

    public GameObject go;

    public GameObject rockGO, treeGO, pathGO, bushGO, fenceGO, crystalGO;

    public List<Node> firstPaths, secondPaths, thirdPaths, fourthPaths;
    public float moveSpeed = 50f; 
    private List<Node> p;
    private int currentPathIndex = 0;

    System.Random random = new System.Random();

    float nodeDiameter;
    int gridSizeX, gridSizeY;

    AdjacencyRules rules;

    Vector3 currTarget;

    private void Awake()
    {
        rules = new AdjacencyRules();
    }
   

    public void CreateTowerDefense()
    {


        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        pathNode = new List<Node>();
        startList = new List<Node>();
        CreateGrid();

        GeneratePath(GetSpecifiedNodePath(NodeType.first), GetSpecifiedStartingPoint(NodeType.first));
        GeneratePath(GetSpecifiedNodePath(NodeType.second), GetSpecifiedStartingPoint(NodeType.second));
        GeneratePath(GetSpecifiedNodePath(NodeType.third), GetSpecifiedStartingPoint(NodeType.third));
        GeneratePath(GetSpecifiedNodePath(NodeType.fourth), GetSpecifiedStartingPoint(NodeType.fourth));

        firstPaths = pf.FindPath(GetSpecifiedStartingPoint(NodeType.first), targetNode, true);
        secondPaths = pf.FindPath(GetSpecifiedStartingPoint(NodeType.second), targetNode, true);
        thirdPaths = pf.FindPath(GetSpecifiedStartingPoint(NodeType.third), targetNode, true);
        fourthPaths = pf.FindPath(GetSpecifiedStartingPoint(NodeType.fourth), targetNode, true);


        go.transform.position = GetSpecifiedStartingPoint(NodeType.first).worldPosition;

        p = firstPaths;

        int path = 0;
        foreach (Node node in grid)
        {
            if (node.isPath)
            {
                path++;
                node.wfcType = WFCType.path;
                node.collapsed = true;
                PlaceInstanceInWorld(node);
            }
        }
        List<Node> markNode = new List<Node>();

        for (int row = 0; row < gridSizeX; row++)
        {
            for (int col = 0; col < gridSizeY; col++)
            {
                if (!grid[row, col].isPath && !grid[row, col].collapsed)
                {
                    markNode.Add(grid[row, col]);
                }
            }
        }
        while (markNode.Count > 0)
        {

            Node currNode = markNode[UnityEngine.Random.Range(0, markNode.Count)];

            currNode.wfcType = rules.GetRandomObjectTypeBasedOnProbabilities(WFCType.none);
            currNode.collapsed = true;

            PlaceInstanceInWorld(currNode);
            markNode.Remove(currNode);

            List<Node> nextNodes = FindNeighbourWFC(currNode);

            if (nextNodes.Count > 0)
            {
                foreach (Node node in nextNodes)
                {
                    //print("Next Node");
                    if (!node.collapsed)
                    {
                        //print("Collapsing");
                        node.possibleTypes.RemoveAll(item => rules.adjacencyInvalidRule[currNode.wfcType].Contains(item));

                        WFCType selectedType;

                        do
                        {
                            selectedType = rules.GetRandomObjectTypeBasedOnProbabilities(currNode.wfcType);
                            //print(selectedType);
                        }
                        while (!node.possibleTypes.Contains(selectedType));


                        node.wfcType = selectedType;
                        node.collapsed = true;

                        PlaceInstanceInWorld(node);

                        markNode.Remove(node);
                        //print(markNode.Count);
                        //print(node.wfcType);
                    }
                }

            }




        }
        GameObject newObj = Instantiate(crystalGO);
        newObj.transform.position = grid[gridSizeX / 2, gridSizeY / 2].worldPosition;
        Terrain terrain = Terrain.activeTerrain;
        if (terrain != null)
        {
            RaycastHit hit;
            if (Physics.Raycast(new Vector3(grid[gridSizeX / 2, gridSizeY / 2].worldPosition.x, 1000f, grid[gridSizeX / 2, gridSizeY / 2].worldPosition.z), Vector3.down, out hit, Mathf.Infinity))
            {
                
                Vector3 terrainPoint = hit.point;
                newObj.transform.position = new Vector3(newObj.transform.position.x, terrainPoint.y, newObj.transform.position.z);

              
                newObj.transform.up = hit.normal;
            }
        }

    }


    public List<Node> FindNeighbourWFC(Node node)
    {
        List<Node> possibleNeighbours = new List<Node> ();
        int numRows = gridSizeX;
        int numCols = gridSizeY;

        if (node.gridY > 0 && grid[node.gridY - 1, node.gridX] != null) possibleNeighbours.Add(grid[node.gridY - 1, node.gridX]);
        if (node.gridY < numRows - 1 && grid[node.gridY + 1, node.gridX] != null)possibleNeighbours.Add(grid[node.gridY + 1, node.gridX]);    
        if (node.gridX < numCols - 1 && grid[node.gridY, node.gridX + 1] != null)possibleNeighbours.Add(grid[node.gridY, node.gridX + 1]);
        if (node.gridX > 0 && grid[node.gridY, node.gridX - 1] != null)possibleNeighbours.Add(grid[node.gridY, node.gridX - 1]);

        return possibleNeighbours;
    }


    public void PlaceInstanceInWorld(Node node)
    {
        GameObject newObj = null;
        WFCType type = node.wfcType;
        if (type == WFCType.none) return;
        if (type == WFCType.tree) newObj = Instantiate(treeGO);
        if (type == WFCType.rock) newObj = Instantiate(rockGO);
        if (type == WFCType.bush) newObj = Instantiate(bushGO);
        if (type == WFCType.fence) newObj = Instantiate(fenceGO);
        if (type == WFCType.path) newObj = Instantiate(pathGO);
        

 
        newObj.transform.position = node.worldPosition;

       
        Terrain terrain = Terrain.activeTerrain;

        if (terrain != null)
        {
            RaycastHit hit;
            if (Physics.Raycast(new Vector3(node.worldPosition.x, 1000f, node.worldPosition.z), Vector3.down, out hit, Mathf.Infinity))
            {
                
                Vector3 terrainPoint = hit.point;
                newObj.transform.position = new Vector3(newObj.transform.position.x, terrainPoint.y, newObj.transform.position.z);

                newObj.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

                
                newObj.transform.Rotate(0, UnityEngine.Random.Range(0, 180), 0);
            }
        }
    }

 

    private void Update()
    {
   
        //go.transform.position = Vector3.MoveTowards(go.transform.position, targetNode.worldPosition, moveSpeed * Time.deltaTime);
        //if (currentPathIndex < p.Count-3)
        //{
        //     float distanceToNextNode = Vector3.Distance(go.transform.position, p[currentPathIndex].worldPosition);

        //    go.transform.position = Vector3.MoveTowards(go.transform.position, p[currentPathIndex].worldPosition, moveSpeed * Time.deltaTime);
            
        //    if (distanceToNextNode < 0.01f)
        //    {
               
        //        currentPathIndex++;
        //    }
        //}
    }


    public void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                worldPoint.y = Terrain.activeTerrain.SampleHeight(worldPoint);



                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));

                if (x == y && !(x == gridSizeX / 2 && y == gridSizeY / 2))
                {
                    grid[x, y] = new Node(NodeType.border, walkable, worldPoint, x, y, false);
                }
                else if (x == gridSizeX - y && !(x == gridSizeX / 2 && y == gridSizeY / 2))
                {
                    grid[x, y] = new Node(NodeType.border, walkable, worldPoint, x, y, false);
                }
                else if (x == gridSizeX/2 && y == gridSizeY / 2)
                {
                    grid[x, y] = new Node(NodeType.second, walkable, worldPoint, x, y, false);
                    targetNode = grid[x, y];
                }


                else if (x <= y && x + y > gridSizeX && !(x == y && (x == gridSizeX - y)))
                {
                    if ((x == 0 && y == gridSizeY / 2) || (x == gridSizeX / 2 && y == 0) || (x == gridSizeX / 2 && y == gridSizeY - 1) || (x == gridSizeX - 1 && y == gridSizeY / 2))
                    {
                        grid[x, y] = new Node(NodeType.fourth, walkable, worldPoint, x, y, true); this.startList.Add(grid[x, y]);
                    }
                    else grid[x, y] = new Node(NodeType.fourth, walkable, worldPoint, x, y, false);
                }
                else if (x > y && x + y <= gridSizeX && !(x == y && (x == gridSizeX - y)))
                {
                    if ((x == 0 && y == gridSizeY / 2) || (x == gridSizeX / 2 && y == 0) || (x == gridSizeX / 2 && y == gridSizeY - 1) || (x == gridSizeX - 1 && y == gridSizeY / 2))
                    {
                        grid[x, y] = new Node(NodeType.first, walkable, worldPoint, x, y, true); this.startList.Add(grid[x, y]);
                    }
                    else grid[x, y] = new Node(NodeType.first, walkable, worldPoint, x, y, false);
                }
                else if (x <= y && x + y <= gridSizeY && !(x == y && (x == gridSizeX - y)))
                {
                    if ((x == 0 && y == gridSizeY / 2) || (x == gridSizeX / 2 && y == 0) || (x == gridSizeX / 2 && y == gridSizeY - 1) || (x == gridSizeX - 1 && y == gridSizeY / 2))
                    {
                        grid[x, y] = new Node(NodeType.third, walkable, worldPoint, x, y, true); this.startList.Add(grid[x, y]);
                    }
                    else grid[x, y] = new Node(NodeType.third, walkable, worldPoint, x, y, false);
                }
                else
                {
                    if ((x == 0 && y == gridSizeY / 2) || (x == gridSizeX / 2 && y == 0) || (x == gridSizeX / 2 && y == gridSizeY - 1) || (x == gridSizeX - 1 && y == gridSizeY / 2))
                    {
                        grid[x, y] = new Node(NodeType.second, walkable, worldPoint, x, y, true); this.startList.Add(grid[x, y]);
                    }
                    else grid[x, y] = new Node(NodeType.second, walkable, worldPoint, x, y, false);
                }
            }
        }


            do
        {
            int xu = UnityEngine.Random.Range(0, gridSizeX);
            int yu = UnityEngine.Random.Range(0, gridSizeY);
            //print(xu + " " + yu);

            Node currentNode = grid[xu, yu];
            if (!currentNode.isPoint && currentNode != null && currentNode.NodeType != NodeType.border && !currentNode.isStartingPoint)
            {
                int nodeTypeCount = pathNode.Count(n => n.NodeType == currentNode.NodeType);
                if (nodeTypeCount < 4)
                {
                    currentNode.isPoint = true;
                    pathNode.Add(currentNode);
                }
            }

            //print(pathNode.Count);
        }
        while (pathNode.Count < 16);
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        worldPosition -= transform.position;
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return grid[x, y];
    }
    public List<Node> path = new List<Node>();
    public List<List<Node>> listOfPaths;

    public int MaxSize
    {
        get
        {
            return gridSizeX * gridSizeY;
        }
    }
    public List<Node> GetSpecifiedNodePath(NodeType nt)
    {
        List<Node> specifiedNodes = new List<Node>();

        foreach (Node node in pathNode)
        {
            if (node.NodeType == nt)
            {
                specifiedNodes.Add(node);
            }
        }

        return specifiedNodes;
    }

    public Node GetSpecifiedStartingPoint(NodeType nodeType)
    {
        foreach (Node node in startList) // line 183
        {
            if (node.NodeType == nodeType)
            {
                return node;
            }
        }

        return null; 
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        if (onlyDisplayPathGizmos)
        {
            if (path != null)
            {
                foreach (Node n in path)
                {
                    Gizmos.color = Color.black;
                    Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
                }
            }
        }
        else
        {

            if (grid != null)
            {
                foreach (Node n in grid)
                {
                    Color color = new Color();
                    if (n.NodeType == NodeType.first)
                    {
                        color = Color.green;
                    }
                    if (n.NodeType == NodeType.second)
                    {
                        color = Color.red;
                    }
                    if (n.NodeType == NodeType.third)
                    {
                        color = Color.blue;
                    }
                    if (n.NodeType == NodeType.fourth)
                    {
                        color = Color.yellow;
                    }
                    if (!n.walkable)
                    {
                        color = Color.black;
                    }
                    if (n.isStartingPoint)
                    {
                        color = Color.white;
                    }
                    if (n.NodeType == NodeType.target)
                    {
                        color = Color.magenta;
                    }

      

                    //if (n.isPoint)
                    //{
                    //    color = Color.white;
                    //}
                    
                    Gizmos.color = color;

                     if (n.isPath) Gizmos.color = Color.white;


                    if (n.isWalkedOn)
                    {
                        Gizmos.color = Color.black;
                    }

                    if (n.isPoint)
                    { 
                        Gizmos.color = Color.white;
                    }
                    Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
                }
            }
    }
}
    private List<Node> GeneratePath(List<Node> nodes, Node startingPoint)
    {
        List<Node> pa = new List<Node>();

        Node current = startingPoint;
        Node nextNode = null;

        do
        {
            nextNode = GetNearestNeighbour(nodes, current);
            if (nextNode == null) break;

            nodes.Remove(current);  
            nodes.Remove(nextNode);  
            pa.AddRange(pf.FindPath(current, nextNode, false));  

            current = nextNode;
        }
        while (nextNode.NodeType != NodeType.target);

        pa.AddRange(pf.FindPath(current, targetNode, false)); 
        return pa;
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




    public void StartDFS()
    {
        StartCoroutine(DFSWithDelay());
    }

    public IEnumerator DFSWithDelay()
    {
        int checker = 0;
        int numRows = gridSizeX;
        int numCols = gridSizeY;
        int randomX, randomY;
        int[] dx = { -1, 1, 0, 0 };
        int[] dy = { 0, 0, -1, 1 };

        Stack<(int, int)> stack = new Stack<(int, int)>();
        bool[,] visited = new bool[numRows, numCols];

        do
        {
            randomX = UnityEngine.Random.Range(0, gridSizeX);
            randomY = UnityEngine.Random.Range(0, gridSizeY);
        } while (grid[randomX, randomY].collapsed);

        stack.Push((randomX, randomY));
        visited[randomX, randomY] = true;

        int notPath = 1;

        while (stack.Count > 0)
        {
            var (x, y) = stack.Pop();

            print($"Visiting cell ({x}, {y})");

            for (int i = 0; i < 4; i++)
            {
                int newX = x + dx[i];
                int newY = y + dy[i];

                if (newX >= 0 && newX < numRows && newY >= 0 && newY < numCols && !visited[newX, newY])
                {
                    Node node = grid[newX, newY];

                    if (node.collapsed)
                    {
                        List<Node> neighbors = new List<Node>();

                        int xo = node.gridX;
                        int yo = node.gridY;

                        int[] dxo = { -1, 1, 0, 0, -1, -1, 1, 1, -2, -2, -2, -1, 1, 2, 2, 2 };
                        int[] dyo = { 0, 0, -1, 1, -1, 1, -1, 1, 0, -1, 1, 2, 2, 2, 1, -1 };

                        for (int p = 0; p < 16; p++)
                        {
                            int newXo = xo + dxo[p];
                            int newYo = yo + dyo[p];

                         
                            if (newXo >= 0 && newXo < numRows && newYo >= 0 && newYo < numCols && !node.collapsed && !visited[newXo, newYo] && !node.isPath)
                            {
                                node = grid[newXo, newYo];
                                node.walkable = false;
                                notPath++;
                                stack.Push((newXo, newYo));
                                visited[newXo, newYo] = true;
                                yield return new WaitForSeconds(.005f);
                            }
                        }
                    }
                    else
                    {
                        node.walkable = false;
                        notPath++;
                        stack.Push((newX, newY));
                        visited[newX, newY] = true;
                        yield return new WaitForSeconds(.005f);
                    }
                }
            }
        }
        print(checker);
    }
    public void StartBFS()
    {
        StartCoroutine(BFSWithDelay());
    }
    public IEnumerator BFSWithDelay()
    {
        int checker = 0;
        int numRows = gridSizeX;
        int numCols = gridSizeY;
        int randomX, randomY;

        do
        {
            randomX = UnityEngine.Random.Range(0, gridSizeX);
            randomY = UnityEngine.Random.Range(0, gridSizeY);
        }
        while (grid[randomX, randomY].collapsed);

        int[] dx = { -1, 1, 0, 0 };
        int[] dy = { 0, 0, -1, 1 };

        Queue<(int, int)> queue = new Queue<(int, int)>();
        bool[,] visited = new bool[numRows, numCols];
        queue.Enqueue((randomX, randomY));
        visited[randomX, randomY] = true;

        if (!grid[randomX, randomY].collapsed)
        {
        }
        int notPath = 1;
        while (queue.Count > 0)
        {
            var (x, y) = queue.Dequeue();



            print($"Visiting cell ({x}, {y})");

            for (int i = 0; i < 4; i++)
            {
                int newX = x + dx[i];
                int newY = y + dy[i];


                if (newX >= 0 && newX < numRows && newY >= 0 && newY < numCols && !visited[newX, newY])
                {
                    Node node = grid[newX, newY];

                    if (node.collapsed)
                    {
                        List<Node> neighbors = new List<Node>();

                        int xo = node.gridX;
                        int yo = node.gridY;

                        int[] dxo = { -1, 1, 0, 0, -1, -1, 1, 1, -2, -2, -2, -1, 1, 2, 2, 2 };
                        int[] dyo = { 0, 0, -1, 1, -1, 1, -1, 1, 0, -1, 1, 2, 2, 2, 1, -1 };

                        for (int p = 0; p < 16; p++)
                        {
                            int newXo = xo + dxo[p];
                            int newYo = yo + dyo[p];

                           
                            //if (newXo >= 0 && newXo < numRows && newYo >= 0 && newYo < numCols && !node.collapsed && !visited[newXo, newYo] && !node.isPath && node.NodeType != NodeType.border)
                            if (newXo >= 0 && newXo < numRows && newYo >= 0 && newYo < numCols && !node.collapsed && !visited[newXo, newYo] && !node.isPath)
                            {
                                node = grid[newXo, newYo];
                                node.walkable = false;
                                notPath++;
                                queue.Enqueue((newXo, newYo));
                                visited[newXo, newYo] = true;
                                yield return new WaitForSeconds(.005f);
                            }
                        }
                    }
                    else
                    //if (!node.collapsed)
                    {
                        node.walkable = false;
                        notPath++;
                        queue.Enqueue((newX, newY));
                        visited[newX, newY] = true;
                        yield return new WaitForSeconds(.005f);
                    }
                }
            }
        }
        print(checker);

        //print($"{gridSizeX * gridSizeY} - {path} = {notPath} => {gridSizeY * gridSizeX - path == notPath}");
    }

    IEnumerator linear()
    {
        List<Node> markNode = new List<Node>();

        for (int row = 0; row < gridSizeX; row++)
        {
            for (int col = 0; col < gridSizeY; col++)
            {
                if (!grid[row, col].isPath && !grid[row, col].collapsed)
                {
                    markNode.Add(grid[row, col]);
                }
            }
        }

        foreach (Node node in markNode)
        {
            node.walkable = false;
            yield return new WaitForSeconds(.005f);
        }
    }





}


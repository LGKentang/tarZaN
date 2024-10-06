using Cinemachine.Utility;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class TDManager : MonoBehaviour
{
    public static bool isStarted = false;
    public static bool isPlaying = false;
    public static int wave;
    public float speedMultiplier = 2, healthMultiplier =  2;
    public GameObject light;
    public List<GameObject> enemies;
    public Grid grid;
    public Canvas TDCanvas;

    public List<Node> spawnPoints;
    public static List<GameObject> npcs;

    public bool IsLose;
    public bool IsWon;

    public static int enemyKilled;
    
    

    public bool hasBeenReseted;
   

    private void Awake()
    {
        spawnPoints = new List<Node>();
        npcs = new List<GameObject>();
        IsLose = false;
        IsWon = false;
        hasBeenReseted = false;
        enemyKilled = 0;
        wave = 1;
        //isStarted = true;
    }

    private void Start()
    {
        RunTowerDefense();
        gameObject.SetActive(false);
        TDCanvas.enabled = false;
    }

    private void Update()
    {
        //print(gameObject.activeSelf);
        if (DialogueManager.isPlay && !TDCanvas.enabled)
        {
            TDCanvas.enabled = true;
        }

        if (isStarted && !isPlaying)
        {
            isPlaying = true;
            AssignMultiplier();
            StartCoroutine(SpawnBy());
        }
        else if (isStarted && isPlaying)
        {
            //print(npcs.Count);
            if (CheckLose()) IsLose = true;

            if (IsLose) ResetTowerDefense();
            else
            {
                if (!IsLose && ValidateWaveDone()) NextWave();
                if (!npcs.Equals(null)) MoveEnemies();
            }
        }

    }

    void NextWave()
    {
        wave++;
        npcs.Clear();
        isStarted = false;
        isPlaying = false;
        IsLose = false;
        IsWon = false;
        speedMultiplier++;
        healthMultiplier++;
        gameObject.SetActive(false);
        TDCanvas.enabled = false;
        //light.SetActive(false); 
    }

    void ResetTowerDefense()
    {
        foreach (GameObject npc in npcs)
        {
            Destroy(npc);
        }

        wave = 1;
        npcs.Clear();
        isStarted = false;
        isPlaying = false;
        IsLose = false;
        IsWon = false;
        speedMultiplier = 2;
        healthMultiplier = 2;
        light.SetActive(false);
        gameObject.SetActive(false);
        hasBeenReseted = true;
        Tower.getInstance().health = 10;
      }


    void RunTowerDefense()
   {
        light.SetActive(true);
        grid.CreateTowerDefense();
        NodeType[] spawnNodeTypes = { NodeType.first, NodeType.second, NodeType.third, NodeType.fourth };

        foreach (NodeType spawnNodeType in spawnNodeTypes)
        {
            spawnPoints.Add(grid.GetSpecifiedStartingPoint(spawnNodeType));
        }
    }

    bool CheckLose()
    {
        return (Tower.getInstance().IsGone());
  
    }

    bool ValidateWaveDone()
    {
        return npcs.Count.Equals(0);
    }

    public int GetEnemyCount()
    {
        return npcs.Count;
    }

    IEnumerator SpawnBy()
    {
        for (int i = 0; i < wave; i++)
        {
            SpawnEnemies();
            yield return new WaitForSeconds(1f);
        }
    }

    void SpawnEnemies()
    {
        Node spawnHere = spawnPoints[Random.Range(0, spawnPoints.Count)];
        GameObject enemyPrefab = enemies[Random.Range(0, enemies.Count)];

        GameObject newEnemy = Instantiate(enemyPrefab, spawnHere.worldPosition, Quaternion.identity);
        TDEnemy tde = newEnemy.GetComponent<TDEnemy>();
        tde.path = GetOptimizedPath(spawnHere.NodeType);
        tde.health = (int)(tde.health * healthMultiplier);
        npcs.Add(newEnemy);
        PositionedCorrectly(newEnemy, spawnHere);
    }

    void MoveEnemies()
    {
        //print("hi");
        List<GameObject> enemiesToRemove = new List<GameObject>();
        //print(npcs.Count);

        foreach (GameObject go in npcs)
        {
            TDEnemy tde = go.GetComponent<TDEnemy>();
            //print(tde == null);
            //print(npcs.IndexOf(go));

            if (tde.currentIndex >= tde.path.Count - 1)
            {
                Tower.getInstance().TakeDamage(1);
                enemiesToRemove.Add(go);
                Destroy(go);

            }
            else
            {
                float distanceToNextNode = Vector3.Distance(go.transform.position, tde.path[tde.currentIndex].worldPosition);
                go.transform.position = Vector3.MoveTowards(go.transform.position, tde.path[tde.currentIndex].worldPosition, Time.deltaTime * speedMultiplier);

                PositionedCorrectly(go, go.transform.position);

                go.transform.LookAt(tde.path[tde.currentIndex+1].worldPosition);

                if (distanceToNextNode < 0.01f)
                {
                    tde.currentIndex++;
                }

            }


        }

        foreach (GameObject enemyToRemove in enemiesToRemove)
        {
            npcs.Remove(enemyToRemove);
        }

    }





    public void AssignMultiplier()
    {
        speedMultiplier += wave * .4f;
        healthMultiplier += wave * .2f;
    }

    public List<Node> GetOptimizedPath(NodeType startingPoint)
    {
        if (startingPoint == NodeType.first) return grid.firstPaths;
        if (startingPoint == NodeType.second) return grid.secondPaths;
        if (startingPoint == NodeType.third) return grid.thirdPaths;
        if (startingPoint == NodeType.fourth) return grid.fourthPaths;
        return null;
    }

    public void PositionedCorrectly(GameObject newObj, Node node)
    {
        newObj.transform.position = node.worldPosition;
        Terrain terrain = Terrain.activeTerrain;
        if (terrain != null)
        {
            RaycastHit hit;
            if (Physics.Raycast(new Vector3(node.worldPosition.x, 100f, node.worldPosition.z), Vector3.down, out hit, Mathf.Infinity))
            {
                Vector3 terrainPoint = hit.point;
                newObj.transform.position = new Vector3(newObj.transform.position.x, terrainPoint.y, newObj.transform.position.z);
                newObj.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            }
        }
    }
    public void PositionedCorrectly(GameObject newObj, Vector3 pos)
    {
    
        Terrain terrain = Terrain.activeTerrain;
        if (terrain != null)
        {
            RaycastHit hit;
            if (Physics.Raycast(new Vector3(pos.x, 1000f, pos.z), Vector3.down, out hit, Mathf.Infinity))
            {
                Vector3 terrainPoint = hit.point;
                newObj.transform.position = new Vector3(newObj.transform.position.x, terrainPoint.y, newObj.transform.position.z);
                newObj.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            }
        }
    }

}

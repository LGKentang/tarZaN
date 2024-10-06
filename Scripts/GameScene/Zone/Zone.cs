using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone : MonoBehaviour
{
    public GameObject enemyPrefab;
    public BoxCollider zone;
    List<GameObject> enemies;
    void Start()
    {
        enemies = new List<GameObject>();
        for (int i = 0; i < 5; i++)
        {
            AddBearRandomInZone();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //print(enemies.Count);
    }

    public void AddBearRandomInZone()
    {
        Vector3 randomSpawnPoint = GetRandomSpawn();

        GameObject newEnemy = Instantiate(enemyPrefab, randomSpawnPoint, Quaternion.identity);
        //print(newEnemy == null ? "null" : "nn");
        AnimalController bc = newEnemy.GetComponent<AnimalController>();

        if (bc != null)
        {
            bc.zone = this.gameObject;
        }


        enemies.Add(newEnemy);
    }


    public Vector3 GetRandomSpawn()
    {
        Bounds bounds = zone.bounds;

        float minX = Mathf.Lerp(bounds.min.x, bounds.max.x, 0.25f);
        float maxX = Mathf.Lerp(bounds.min.x, bounds.max.x, 0.75f); 
        float minZ = Mathf.Lerp(bounds.min.z, bounds.max.z, 0.25f); 
        float maxZ = Mathf.Lerp(bounds.min.z, bounds.max.z, 0.75f); 

        float randomX = Random.Range(minX, maxX);
        float randomZ = Random.Range(minZ, maxZ);

        Vector3 randomSpawnPoint = new Vector3(randomX, Terrain.activeTerrain.SampleHeight(new Vector3(randomX, 0, randomZ)), randomZ);

        return randomSpawnPoint;
    }
}

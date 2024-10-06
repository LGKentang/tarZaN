using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collideTest : MonoBehaviour
{
    public BoxCollider collider;
    public GameObject blackSpherePrefab; // Assign your black sphere prefab in the Inspector

    private float spawnInterval = 0.2f; // Time interval between spawns
    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            // Instantiate a black sphere at a random point within the bounds of the collider
            if (collider != null && blackSpherePrefab != null)
            {
                Vector3 minBounds = collider.bounds.min;
                Vector3 maxBounds = collider.bounds.max;

                float randomX = Random.Range(minBounds.x, maxBounds.x);
                float randomZ = Random.Range(minBounds.z, maxBounds.z);

                Vector3 randomPoint = new Vector3(randomX, Terrain.activeTerrain.SampleHeight(new Vector3(randomX,0,randomZ)), randomZ);

                // Create a black sphere GameObject at the random point
                InstantiateObject(randomPoint);

                // Reset the timer
                timer = 0f;
            }
            else
            {
                Debug.LogError("Collider or blackSpherePrefab is not assigned!");
            }
        }
    }

    private void InstantiateObject(Vector3 position)
    {
        // Instantiate the black sphere at the specified position
        Instantiate(blackSpherePrefab, position, Quaternion.identity);
    }
}

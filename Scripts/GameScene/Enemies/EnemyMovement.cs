using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Adjust this to control the speed of the enemy
    private List<Node> path;
    private int currentPathIndex = 0;

    void Start()
    {
        // Assign the firstPaths list to the path variable
        path = GetComponent<Grid>().firstPaths;
    }

    void Update()
    {
        // Check if there are nodes in the path
        if (currentPathIndex < path.Count)
        {
            // Calculate the distance between the enemy and the next node in the path
            float distanceToNextNode = Vector3.Distance(transform.position, path[currentPathIndex].worldPosition);

            // Move towards the next node
            transform.position = Vector3.MoveTowards(transform.position, path[currentPathIndex].worldPosition, moveSpeed * Time.deltaTime);

            // Check if the enemy has reached the next node
            if (distanceToNextNode < 0.1f)
            {
                // Increment the current path index to move to the next node
                currentPathIndex++;
            }
        }
    }
}

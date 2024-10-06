using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonWalkSound : MonoBehaviour
{
    public List<AudioClip> walkSound;
    public List<AudioClip> attackSound;
    public AudioSource audioSource;
    public int pos;

    public static bool playerIsInArea;

    public AudioClip ropeSound;

    private void Start()
    {
        playerIsInArea = false;
    }

    public void dragonStep()
    {
        if (playerIsInArea) {
            pos = (int)Mathf.Floor(Random.Range(0, walkSound.Count));
            audioSource.PlayOneShot(walkSound[pos]);
        }
        
    }

    public void dragonAttack()
    {
        pos = (int)Mathf.Floor(Random.Range(0, attackSound.Count));
        audioSource.PlayOneShot(attackSound[pos]);
    }
}

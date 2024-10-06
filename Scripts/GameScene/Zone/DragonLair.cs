using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DragonLair : MonoBehaviour
{
    public AudioSource mainMusic;
    public AudioSource intenseMusic;

    private void Start()
    {
        mainMusic.Play();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (DragonController.isAbleToSpawn && other.gameObject.CompareTag("Player"))
        {
            DragonWalkSound.playerIsInArea = true;  
            print("Entering dragon lair");
            mainMusic.Pause();
            intenseMusic.Play();
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            intenseMusic.Pause();
            mainMusic.UnPause();
            DragonWalkSound.playerIsInArea = false;
        }
    }

}

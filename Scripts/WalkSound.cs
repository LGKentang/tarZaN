using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkSound : MonoBehaviour
{
    public List<AudioClip> walkSound;
    public List<AudioClip> rockSound;
    public List<AudioClip> puddleSound;
    public List<AudioClip> punchingSound;
    public AudioSource audioSource;
    public int pos;
    public int posPuddle;
    public int posRock;
    public int posPunch;

    public AudioClip ropeSound;

    public static bool isPuddleSound;
    public static bool isRockGroundSound;

    public AudioClip jumpS;


    void Start()
    {
        isPuddleSound = false;    
        isRockGroundSound = false;
    }

    public void punchSound()
    {
        posPunch = (int)Mathf.Floor(Random.Range(0, punchingSound.Count));
        audioSource.PlayOneShot(punchingSound[pos]);
    }

    public void jumpSound()
    {
        audioSource.PlayOneShot(jumpS);
    }

    public void playSound()
    {
        if (!isPuddleSound &&  !isRockGroundSound)
        {
            pos = (int) Mathf.Floor(Random.Range(0,walkSound.Count));
            audioSource.PlayOneShot(walkSound[pos]);
        }

        if (isPuddleSound)
        {
           posPuddle = (int)Mathf.Floor(Random.Range(0, puddleSound.Count));
            audioSource.PlayOneShot(puddleSound[posPuddle]);
        }

        if (isRockGroundSound)
        {
            posRock  =(int)Mathf.Floor(Random.Range(0,rockSound.Count));
            audioSource.PlayOneShot(rockSound[posRock]);        
        }
    }
}

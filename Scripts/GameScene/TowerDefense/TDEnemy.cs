using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TDEnemy : MonoBehaviour
{
    public HealthBar hb;
    public int currentIndex = 0;
    public int health = 20;
    public List<Node> path;
    

    private void Start()
    {
       gameObject.tag = "TDEnemy";
       gameObject.layer = LayerMask.NameToLayer("TDEnemy");
       hb.SetMaxHealth(health);    
    }
    private void Update()
    {

        hb.SetHealth(health);

        if (IsDead()) 
        {
            GameObject go = gameObject;
            Destroy(gameObject);
            TDManager.npcs.Remove(go);
        }
    }

    public bool IsDead()
    {
        return health <= 0;
    }

    public void TakeDamage(int value)
    {
        health -= value;
    }

    
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Tower
{
    public static Tower towerInstance = null;
    public float health;

    private Tower(float health) {
        this.health = health;
    }

    public void ResetHealth()
    {
        health = 10f;
    }

    public static Tower getInstance()
    {
        if (towerInstance == null)
        {
            towerInstance = new Tower(10);
        }
        return towerInstance;
    }

    public void TakeDamage(int value) {
        health -= value;
    }

    public bool IsGone()
    {
        return health <= 0;
    }

}

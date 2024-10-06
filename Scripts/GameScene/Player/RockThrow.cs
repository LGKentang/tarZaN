using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RockThrow : MonoBehaviour
{
    public Collider collider;
    int damage = 20;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bear"))
        {
            //print("Trigger");
            AnimalController bear = other.GetComponent<AnimalController>();
            if (bear != null && !bear.becomePet)
            {
                bear.TakeDamage(damage);
                
                if (bear.becomePet)
                {
                    //print("Add XP");
                    PlayerAttributes.GetInstance().AddExperience(bear.health);
                }
            }
        }

        if (other.CompareTag("Dragon"))
        {
            //print("Trigger");
            DragonController dg = other.GetComponent<DragonController>();
            if (dg != null & !dg.isDead)
            {
                dg.TakeDamage(damage);

                if (dg.isDead)
                {
                    EndScene.IsWon = true;
                    SceneManager.LoadScene("EndScene");
                    print("Add XP");
                    PlayerAttributes.GetInstance().AddExperience(dg.health);
                }
            }
        }

        if (other.CompareTag("TDEnemy"))
        {
            //print("Trigger");
            TDEnemy tde = other.GetComponent<TDEnemy>();    
            if (tde.gameObject != null)
            {
                print(tde.health);
                tde.TakeDamage(damage);
                
                if (tde.gameObject == null || tde.IsDead())
                {
                    TDManager.enemyKilled++;
                    print("death");
                    PlayerAttributes.GetInstance().AddExperience(10);
                }
            }
        }


    }
}

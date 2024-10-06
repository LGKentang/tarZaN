using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAttributes
{
    public float Health { get; set; }
    public float Experience { get; set; }
    public float Attack { get; set; }
    public float Defense { get; set; }
    public int Level { get; set; }

    public List<GameObject> Pets { get; set; }

    public bool hasWon;

 
    public float MaxHealth { get; set; }
    public float MaxExperience { get;set; }
    

    public static PlayerAttributes playerAttributesInstance;

    private PlayerAttributes() {
        MaxHealth = 100f;
        MaxExperience = 50f;
        Pets = new List<GameObject>();
        hasWon = false;
    }

 

    public static PlayerAttributes GetInstance()
    {
        if (playerAttributesInstance == null)
        {
              playerAttributesInstance = new PlayerAttributes();
        }
        return playerAttributesInstance;
    }



    public int getPetCount()
    {
        return Pets.Count;
    }
    
    public void OnLevelUp()
    {
        MaxHealth += 10;
        MaxExperience += 20;

        Health = MaxHealth;
        Attack += 3f;
        Defense += 3f;

        Level++;
        Experience -= MaxExperience;
    }

    public bool ValidateLevelUp()
    {
        return Experience >= MaxExperience;
    }

    public void AddPet(GameObject go)
    {
        Pets.Add(go); 
    }

    public void AddExperience(float xp)
    {
        Experience += xp;
    }
    
    public bool IsPet(GameObject go)
    {
        return Pets.Contains(go);
    }

}

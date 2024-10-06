using System.Collections;
using System.Collections.Generic;
using TMPro;
//using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Cinemachine.DocumentationSortingAttribute;

public class SetupEnv : MonoBehaviour
{
    PlayerAttributes playerAttributes = PlayerAttributes.GetInstance();
    public HealthBar healthBar;
    public HealthBar xpBar;
    public TextMeshProUGUI petCount;
    public Text level;
    public bool isDragonSpawn;
    public GameObject dragon;
    public GameObject lair;

    private void Start()
    {
        SetPlayerStartingAttributes();
        AssignValueToBars();
        isDragonSpawn = false;
    }

    private void Update()
    {
        healthBar.SetHealth(playerAttributes.Health);
        xpBar.SetHealth(playerAttributes.Experience);
        level.text = playerAttributes.Level.ToString();
        petCount.text = playerAttributes.getPetCount().ToString();


        if (playerAttributes.Level > 1 && !isDragonSpawn)
        {
            print("Lvl up");
            lair.SetActive(false);
            isDragonSpawn = true;
            dragon.SetActive(true);
            DragonController.isAbleToSpawn = true;  
        }


        if (playerAttributes.ValidateLevelUp())
        {
            LevelUpHandler();
        }

        if (playerAttributes.Health < 0)
        {
            EndScene.IsWon = false;
            SceneManager.LoadScene("EndScene");
        }
    }

    public void LevelUpHandler()
    {
       
        playerAttributes.MaxHealth += 10;
        healthBar.SetMaxHealth(playerAttributes.MaxHealth);
        playerAttributes.MaxExperience += 20;
        xpBar.SetMaxHealth(playerAttributes.MaxExperience);

        playerAttributes.Health = playerAttributes.MaxHealth;
        playerAttributes.Attack += 3f;
        playerAttributes.Defense += 3f;

        playerAttributes.Level++;
        playerAttributes.Experience -= playerAttributes.MaxExperience;
    }


    private void SetPlayerStartingAttributes()
    {
        playerAttributes.Level = 1;
        playerAttributes.Health = playerAttributes.MaxHealth;
        playerAttributes.Attack = 10f;
        playerAttributes.Experience = 0f;
        playerAttributes.Defense = 5f;
    }

    private void AssignValueToBars()
    {
        healthBar.SetMaxHealth(playerAttributes.MaxHealth);
        xpBar.SetMaxHealth(playerAttributes.MaxExperience);
        xpBar.SetHealth(0f);
        level.text = playerAttributes.Level.ToString();
    }

}

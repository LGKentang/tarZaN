using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TowerManager : MonoBehaviour
{
    public TowerHealth hb;
    public TextMeshProUGUI wave;
    public TextMeshProUGUI enemiesCount;

    void Start()
    {
        hb.SetMaxHealth(Tower.getInstance().health);
    }

    void Update()
    {
        
        hb.SetHealth(Tower.getInstance().health);
        //if (TDManager.isPlaying) { 
            wave.text ="Wave " + TDManager.wave.ToString();
            enemiesCount.text = TDManager.npcs.Count.ToString() + "/" + TDManager.wave.ToString() + " enemies";    
        //}
        //else
        //{
        //    wave.text = "None";
        //    enemiesCount.text = "None";
        //}
    }
}

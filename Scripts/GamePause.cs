using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GamePause : MonoBehaviour
{
    public void goToStart()
    {
        SceneManager.LoadScene("StartScene");
        Time.timeScale = 1f;
    }
}

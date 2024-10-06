using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScene : MonoBehaviour
{
    public static bool? IsWon;
    public string victoryDescription = "Inspired by Passion, Driven by Purpose, Together we shatter limits and redefine boundaries - Bluejacket 23-2";
    public string defeatDescription = "Wonderful things can be achieved when there are teamwork, hardwork, and perseverance - Bluejacket 22-2";
    public TextMeshProUGUI vicOrDef;
    public TextMeshProUGUI description;

    private void Start()
    {
        print(IsWon);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        if (IsWon != null)
        {
            if (IsWon == true) {
                vicOrDef.text = "Victory";
                description.text = victoryDescription;
            }
            else
            {
                vicOrDef.text = "Defeat";
                description.text = defeatDescription;       
            }
        }
    }


    public void GoToMainMenu()
    {
        print("going");
        SceneManager.LoadScene("StartScene");
    }

}

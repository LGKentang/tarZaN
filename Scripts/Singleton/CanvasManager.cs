using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public Canvas fpsCanvas;
    public Canvas dialogueCanvas;

    public static Canvas fpsStatic;
    public static Canvas dialogueStatic;

    private void Awake()
    {
       fpsStatic = fpsCanvas;
        dialogueStatic = dialogueCanvas;

       fpsStatic.gameObject.SetActive(true);
        dialogueStatic.gameObject.SetActive(false);
    }

    public static void ChangeToCanvas(Canvas a)
    {
    
        fpsStatic.gameObject.SetActive(false);
        dialogueStatic.gameObject.SetActive(false);

   
        a.gameObject.SetActive(true);
        
    }
}

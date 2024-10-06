using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    public static bool isDialoguing;
    public Animator animator;
    public Dialogue dialogue;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            animator.SetTrigger("waveTrigger");
            //print("Enter");
            CanvasManager.ChangeToCanvas(CanvasManager.dialogueStatic);
            PlayerMovement.isNPCinteractable = true;
            isDialoguing = true; Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //print("Exiting");
        if (other.CompareTag("Player"))
        {
            //print("Player exit");
            CanvasManager.ChangeToCanvas(CanvasManager.fpsStatic);
            PlayerMovement.isNPCinteractable = false;
            Cursor.visible = false; 
        }
    }


    void Start()
    {
        isDialoguing = false; 
    }

    public static void ToggleInteraction()
    {
        CanvasManager.ChangeToCanvas(CanvasManager.dialogueStatic);
       
    }

}

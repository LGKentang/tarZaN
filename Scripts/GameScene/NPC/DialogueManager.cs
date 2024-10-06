using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour {

	
	public TextMeshProUGUI nameText;
	public TextMeshProUGUI dialogueText;
	public Button yesButton;
	public Button noButton;
	public Button continueButton;
	public static bool isOption;
	public static bool isPlay;
	bool isAnswered;

	public GameObject td;

	//public Animator animator;

	public Queue<string> sentences;

	
	void Start () {
		yesButton.gameObject.SetActive(false);
		noButton.gameObject.SetActive(false);
        Cursor.visible = true;
		isOption = false;
		isAnswered = false;
		sentences = new Queue<string>();
		isPlay = false;
	}

    private void Update()
    {
        if (!isOption)
        {
			continueButton.gameObject.SetActive(true);
            yesButton.gameObject.SetActive(false);
            noButton.gameObject.SetActive(false);
        }
        if (isOption && !isAnswered) {
            if (Input.GetKeyDown(KeyCode.Y))
            {
				AcceptTowerDefense();
            }
            if (Input.GetKeyDown(KeyCode.N))
            {
				DeclineTowerDefense();
            }
        }	
		
    }

	public void AcceptTowerDefense()
	{
		GetOutOfCanvas();
        print("Starting game");
		td.SetActive(true);
		TDManager.isStarted = true;
		isPlay = true;
	}

	public void DeclineTowerDefense()
	{
		GetOutOfCanvas();
    }

	public void GetOutOfCanvas()
	{
        isAnswered = true;
        CanvasManager.ChangeToCanvas(CanvasManager.fpsStatic);
        PlayerMovement.isNPCinteractable = true;
        Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
		isOption = false;
    }

    public void StartDialogue (Dialogue dialogue)
	{
		//animator.SetBool("IsOpen", true);
		isAnswered = false;
		nameText.text = dialogue.name;

		sentences.Clear();

		foreach (string sentence in dialogue.sentences)
		{
			sentences.Enqueue(sentence);
		}

		DisplayNextSentence();
	}

	public void DisplayNextSentence ()
	{
		string sentence = sentences.Dequeue();
		StopAllCoroutines();
		StartCoroutine(TypeSentence(sentence));
		if (sentences.Count == 0)
		{
			
            EndDialogue();
			return;
		}

	}

	IEnumerator TypeSentence (string sentence)
	{
		dialogueText.text = "";
		foreach (char letter in sentence.ToCharArray())
		{
			dialogueText.text += letter;
			yield return null;
		}
	}

	void EndDialogue()
	{
		isOption = true;
        continueButton.gameObject.SetActive(false);
        yesButton.gameObject.SetActive(true);
        noButton.gameObject.SetActive(true);

	}



}

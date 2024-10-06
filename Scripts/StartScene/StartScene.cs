using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartScene : MonoBehaviour
{
    public List<Sprite> qualitySprites;
    public Image qualityImage;

    public Canvas startCanvas, loadingCanvas, settingCanvas;
    public AudioMixer mainMixer;
    public AudioSource woodSource, settingSource, exitSource, metalSource, bgSource, natureSource;
    public Slider loadSlider;

    public TextMeshProUGUI loadingMessage;
    public Animator fadeTransition,fadeExit, darkExit;

    private void Start()
    {
        settingCanvas.enabled = false;
        loadingCanvas.enabled = false;  
        SetAntiAlias(3);

        qualityImage.sprite = qualitySprites[2];
    }

    IEnumerator LoadLevelAsync(string level)
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(level);

        while (!loadOperation.isDone)
        {
            float progressValue = Mathf.Clamp01(loadOperation.progress / .9f);
            loadSlider.value = progressValue;
            yield return null;
        }
        
    }

   IEnumerator FadeTransition()
    {
        fadeTransition.SetTrigger("start");
        yield return new WaitForSeconds(1f); 
        loadingCanvas.enabled = true;
        StartCoroutine(LoadLevelAsync("SampleScene"));
    }

    public void SetAntiAlias(int index)
    {
        print(index);

        int[] antiAliasingValues = { 0, 2, 4, 8 };
   
        QualitySettings.antiAliasing = antiAliasingValues[index];
      
    }


    public void SetVolume(float volume)
    {
        mainMixer.SetFloat("volume",Mathf.Log10(volume)*20);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetQuality(int index)
    {
        QualitySettings.SetQualityLevel(index);
        qualityImage.sprite = qualitySprites[index];
    }

    public void ChangeToSetting()
    {
        metalSource.Play();
        settingCanvas.enabled = true;
        startCanvas.enabled = false;
    }

    public void ChangeToMenu()
    {
        settingCanvas.enabled = false;
        startCanvas.enabled = true; 
    }

    public void OnPlayClick()
    {
        loadingMessage.text = loadingMessages[Random.Range(0, loadingMessages.Length)];
        woodSource.Play();
        startCanvas.enabled = false; 
        StartCoroutine(FadeTransition());
    }
    private IEnumerator FadeOutAudio(AudioSource audioSource, float duration)
    {
        float startVolume = bgSource.volume;
        float startVolume2 = natureSource.volume;

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            bgSource.volume = Mathf.Lerp(startVolume, 0f, elapsedTime / duration);
            natureSource.volume = Mathf.Lerp(startVolume2, 0f, elapsedTime / duration);
            yield return null;
        }


        audioSource.volume = 0f;
        audioSource.Stop();
    }
    IEnumerator ExitGameTransition()
    {
        fadeExit.SetTrigger("endTrigger");
        darkExit.SetTrigger("startTrigger");
        print("exited");

        yield return new WaitForSeconds(1f);
        Application.Quit();
    }

    public void ExitGame()
    {
        
        exitSource.Play();
        StartCoroutine(ExitGameTransition());
        
    }

    string[] loadingMessages = {
    "Preparing the jungle for action...",
    "Exploring the wild for resources...",
    "Crafting tools for defense...",
    "Gearing up for the challenges ahead...",
    "The jungle's secrets await...",
    "Fortifying your base for safety...",
    "New threats on the horizon...",
    "Building a strong defense strategy...",
    "Wildlife allies at your service...",
    "Unlocking the jungle's potential...",
    "Raising the stakes for survival...",
    "Evolving your tactics for victory...",
    "Strengthening your position in the jungle...",
    "Jungle guardians on alert...",
    "Harnessing nature's power for protection...",
    "Adventuring deeper into the wild...",
    "Ready for the next wave of challenges...",
    "Adapting to the ever-changing jungle...",
    "New horizons, new obstacles...",
    "Mastery of the jungle's mysteries...",
    "Gathering essential supplies...",
    "The jungle's beauty and danger...",
    "Facing the unknown with courage...",
    "Nature's strength at your disposal...",
    "Defending against jungle invaders...",
    "Resilience in the face of adversity...",
    "Survival of the fittest in the jungle...",
    "Strategizing for success in the wild...",
    "Jungle allies await your command...",
    "Discovering the heart of the wilderness...",
    "New adventures, new challenges...",
    "The wild calls for your expertise...",
    "Navigating the jungle's obstacles...",
    "Adventurers, the journey continues...",
    "Safety in the midst of the jungle...",
    "Jungle mastery, one step at a time...",
    "Staying vigilant in the wild...",
    "Nature's wonders and dangers...",
    "Progressing toward your goals...",
    "Building a stronghold in the jungle..."
};



}

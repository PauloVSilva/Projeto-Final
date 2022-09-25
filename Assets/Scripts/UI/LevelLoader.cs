using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LevelLoader : MonoBehaviour{
    public GameObject loadingScreen;
    public Slider slider;
    public TextMeshProUGUI percentage;
    private float progress;

    private void Awake(){
        InitializeVariables();
    }

    private void InitializeVariables(){
        progress = 0f;
    }

    public void LoadLevel(string sceneName){
        StartCoroutine(LoadAsynchronously(sceneName));
        loadingScreen.SetActive(true);
    }

    IEnumerator LoadAsynchronously(string sceneName){
        yield return new WaitForSeconds(1f);
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        while(!operation.isDone){
            progress = Mathf.Clamp01(operation.progress / .9f);
            //Debug.Log(progress);
            slider.value = progress;
            percentage.text = progress * 100f + "%";
            yield return null;
        }
        if(operation.isDone){
            loadingScreen.SetActive(false);

            progress = 0f;
            slider.value = progress;
            percentage.text = progress + "%";

            if(sceneName == "MainMenu"){
                Destroy(GameManager.instance.gameObject);
                //SceneManager.UnloadSceneAsync("MainHub");
            }
        }
    }
}

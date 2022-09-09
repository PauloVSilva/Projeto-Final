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

    public void LoadLevel(string sceneName){
        StartCoroutine(LoadAsynchronously(sceneName));
    }

    IEnumerator LoadAsynchronously(string sceneName){
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        loadingScreen.SetActive(true);
        while(!operation.isDone){
            float progress = Mathf.Clamp01(operation.progress / .9f);
            Debug.Log(progress);
            slider.value = progress;
            percentage.text = progress * 100f + "%";
            yield return null;
        }
        if(operation.isDone){
            loadingScreen.SetActive(false);
        }
    }
}

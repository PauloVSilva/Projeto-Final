using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LevelLoader : MonoBehaviour{
    public static LevelLoader instance = null;
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI percentage;
    [SerializeField] private TextMeshProUGUI loading;
    private string loadingString;
    private float progress;

    public event System.Action OnSceneLoaded;

    private void Awake(){
        InitializeSingletonInstance();
        InitializeVariables();
    }

    private void InitializeSingletonInstance(){
        if(instance == null){
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if(instance != null){
            Destroy(gameObject);
        }
    }

    private void InitializeVariables(){
        loadingString = "Loading";
        progress = 0f;
    }

    public void LoadLevel(string sceneName){
        GameManager.Instance.mainCamera = null;
        StartCoroutine(LoadAsynchronously(sceneName));
        loading.text = loadingString;
        loadingScreen.SetActive(true);
    }

    IEnumerator LoadAsynchronously(string sceneName){
        yield return new WaitForSeconds(1f);
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        while(!operation.isDone){
            progress = Mathf.Clamp01(operation.progress / .9f);
            progress = Mathf.Round(progress * 100f);
            slider.value = progress;
            percentage.text = progress + "%";
            yield return null;
        }
        if(operation.isDone){
            OnSceneLoaded?.Invoke();

            loadingScreen.SetActive(false);

            progress = 0f;
            slider.value = progress;
            percentage.text = progress + "%";
        }
    }

    public void QuitGame(){
        Debug.Log("QUIT!");
        Application.Quit();
    }
}

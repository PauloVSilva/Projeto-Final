using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LevelLoader : Singleton<LevelLoader>
{
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI percentage;
    [SerializeField] private TextMeshProUGUI loading;
    private string loadingString;
    private float progress;

    [SerializeField] private string currentSceneName;
    [SerializeField] private int currentSceneID;

    public event System.Action OnSceneLoaded;

    protected override void Awake()
    {
        base.Awake();
        InitializeVariables();
    }

    private void InitializeVariables()
    {
        loadingString = "Loading";
        progress = 0f;
    }

    public void LoadLevel(string sceneName)
    {
        GameManager.Instance.mainCamera = null;
        StartCoroutine(LoadAsynchronously(sceneName, true));
        loading.text = loadingString;
        loadingScreen.SetActive(true);
    }

    IEnumerator LoadAsynchronously(string sceneName, bool replaceCurrent)
    {
        yield return new WaitForSeconds(0f);

        if (replaceCurrent && currentSceneName != "")
        {
            AsyncOperation subOperation = SceneManager.UnloadSceneAsync(currentSceneName);

            while (!subOperation.isDone)
            {
                yield return null;
            }
            if (subOperation.isDone)
            {
                currentSceneName = sceneName;
            }
        }
        else
        {
            currentSceneName = sceneName;
        }

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        while (!operation.isDone)
        {
            progress = Mathf.Clamp01(operation.progress / .9f);
            progress = Mathf.Round(progress * 100f);
            slider.value = progress;
            percentage.text = progress + "%";
            yield return null;
        }

        if (operation.isDone)
        {
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

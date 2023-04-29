using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class MainMenuManager : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip mainTheme;

    private void OnEnable()
    {
        InitializeMainMenu();
    }

    private void Update()
    {
        PlayTheme();
    }

    private void PlayTheme()
    {
        if(!audioSource.isPlaying) audioSource.Play();
    }

    private void InitializeMainMenu()
    {
        GameManager.Instance.UpdateGameState(GameState.MainMenu);
        CanvasManager.Instance.OpenMenu(Menu.MainMenu);

        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.clip = mainTheme;
        audioSource.volume = 0.5f;
    }
}
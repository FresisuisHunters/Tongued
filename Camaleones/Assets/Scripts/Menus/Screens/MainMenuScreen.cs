using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Audio;

#pragma warning disable 649
[RequireComponent(typeof(CanvasGroup))]
public class MainMenuScreen : AMenuScreen
{
    [SerializeField] private SceneReference multiplayerLobbyScene;
    [SerializeField] private AudioMixer audioMixer;

    protected override void OnOpen(Type previousScreen) {
        Settings.LoadSettings(audioMixer);
    }

    public void GoToMultiplayerLobby()
    {
        SceneManager.LoadScene(multiplayerLobbyScene, LoadSceneMode.Single);
    }

    public void GoToTraining()
    {
        MenuManager.SetActiveMenuScreen<TrainingMenuScreen>();
    }

    public void GoToSettings()
    {
        MenuManager.SetActiveMenuScreen<SettingsMenuScreen>();
    }

    public void GoToCredits()
    {
        MenuManager.SetActiveMenuScreen<CreditsMenuScren>();
    }

}

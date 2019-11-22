using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#pragma warning disable 649
[RequireComponent(typeof(CanvasGroup))]
public class MainMenuScreen : AMenuScreen
{
    [SerializeField] private SceneReference multiplayerLobbyScene;

    #region Overrides
    protected override void OnClose(System.Type nextScreen)
    {

    }

    protected override void OnOpen(System.Type previousScreen)
    {

    }
    #endregion

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

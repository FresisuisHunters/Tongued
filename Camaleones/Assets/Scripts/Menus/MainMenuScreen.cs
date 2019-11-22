using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#pragma warning disable 649
public class MainMenuScreen : AMenuScreen
{
    [SerializeField] private SceneReference multiplayerLobbyScene;

    #region Overrides
    public override void OnClose()
    {

    }

    public override void OnOpen()
    {

    }
    #endregion

    public void GoToMultiplayerLobby()
    {
        SceneManager.LoadScene(multiplayerLobbyScene, LoadSceneMode.Single);
    }

    public void GoToTraining()
    {
        //MenuManager.SetActiveMenuScreen()
    }

    public void GoToSettings()
    {
        //Scene
    }

    public void GoToCredits()
    {

    }

}

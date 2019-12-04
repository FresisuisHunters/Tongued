using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AskUsernameScreen : AMenuScreen
{
    [SerializeField] private SceneReference mainMenuScene;
    [SerializeField] private TMPro.TMP_InputField usernameInputField;

    public override void GoBack()
    {
        SceneManagerExtensions.LoadScene(mainMenuScene, LoadSceneMode.Single, () =>
            FindObjectOfType<MenuScreenManager>().startingMenuScreen = FindObjectOfType<MainMenuScreen>());
    }

    public void GoToLobbyScreen()
    {
        MenuManager.SetActiveMenuScreen<LobbyScreen>();
    }

    public void ConnectToPhotonServer()
    {
        string username = usernameInputField.text;
        if (string.IsNullOrEmpty(username) || string.IsNullOrWhiteSpace(username))
        {
            return;
        }
        PhotonNetwork.LocalPlayer.NickName = username;

        if (PhotonNetwork.IsConnected)
        {
            OnlineLogging.Instance.Write("Player already connected to Photon server");
            return;
        }

        PhotonNetwork.GameVersion = ServerConstants.GAME_VERSION;
        PhotonNetwork.ConnectUsingSettings();

        SetInteractable(false);
    }
}

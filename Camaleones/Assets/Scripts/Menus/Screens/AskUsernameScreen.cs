﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System;
using TMPro;

#pragma warning disable 649
public class AskUsernameScreen : AMenuScreen, IConnectionCallbacks
{
    #region Inspector
    [SerializeField] private SceneReference mainMenuScene;
    [SerializeField] private TMP_InputField usernameInputField;
    [SerializeField] private Button connectToServerButton;
    [SerializeField] private TextMeshProUGUI messageField;
    [SerializeField] private Button backButton;
    #endregion

    protected override void OnOpen(Type previousScreen) {
        PhotonNetwork.AddCallbackTarget(this);

        if (previousScreen == typeof(LobbyScreen) && Settings.IS_PHONE)
        {
            GoBack();
            return;
        }

        messageField.text = "Welcome to Tongued Online";

        if (PhotonNetwork.IsConnectedAndReady) {
            GoToLobbyScreen();
        }

        if (Settings.IS_PHONE) {
            usernameInputField.text = TonguedUsernamesBank.RetrieveUsername();
            HideInterface();
            ConnectToServer();
        }
    }
    protected override void OnClose(Type nextScreen) => PhotonNetwork.RemoveCallbackTarget(this);

    public override void GoBack()
    {
        PhotonNetwork.Disconnect();
        SceneManagerExtensions.LoadScene(mainMenuScene, LoadSceneMode.Single, () =>
            FindObjectOfType<MenuScreenManager>().startingMenuScreen = FindObjectOfType<MainMenuScreen>());
    }

    private void HideInterface() {
        usernameInputField.gameObject.SetActive(false);
        connectToServerButton.gameObject.SetActive(false);
    }

    public void ConnectToServer()
    {
        string username = usernameInputField.text;
        if (string.IsNullOrEmpty(username) || string.IsNullOrWhiteSpace(username))
        {
            return;
        }

        if (PhotonNetwork.IsConnected)
        {
            OnlineLogging.Instance.Write("Player already connected to Photon server");
            return;
        }

        PhotonNetwork.LocalPlayer.NickName = username;
        PhotonNetwork.GameVersion = ServerConstants.GAME_VERSION;
        PhotonNetwork.ConnectUsingSettings();

        messageField.text = "Connecting to server...";
        SetInteractable(false, backButton);
    }

    #region Connection Callbacks
    void IConnectionCallbacks.OnConnectedToMaster() => GoToLobbyScreen();

    void IConnectionCallbacks.OnConnected() { }
    void IConnectionCallbacks.OnDisconnected(DisconnectCause cause) { }
    void IConnectionCallbacks.OnRegionListReceived(RegionHandler regionHandler) { }
    void IConnectionCallbacks.OnCustomAuthenticationResponse(Dictionary<string, object> data) { }
    void IConnectionCallbacks.OnCustomAuthenticationFailed(string debugMessage) { }

    #endregion

    private void GoToLobbyScreen()
    {
        MenuManager.SetActiveMenuScreen<LobbyScreen>();
    }
}

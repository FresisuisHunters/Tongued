﻿using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

#pragma warning disable 649
public class OnlineLobbyManager : MonoBehaviourPunCallbacks {

    #region Constant Fields

    private const string CONNECTION_STATUS_MESSAGE = "Connection status:\n";

    #endregion

    #region Private Fields

    private static OnlineLobbyManager instance;
    private GameObject activePanel;
    [SerializeField] private TextMeshProUGUI connectionStatusText;
    [SerializeField] private GameObject askUsernamePanel;
    [SerializeField] private GameObject lobbyMenuPanel;
    [SerializeField] private GameObject createRoomPanel;
    [SerializeField] private GameObject joinPrivateRoomPanel;
    [SerializeField] private GameObject roomPanel;

    #endregion

    #region Unity Callbacks

    private void Awake () {
        PhotonNetwork.AutomaticallySyncScene = true;

        activePanel = askUsernamePanel;
        instance = this;
    }

    private void Update () {
        connectionStatusText.text = CONNECTION_STATUS_MESSAGE + PhotonNetwork.NetworkClientState;
    }

    protected void OnApplicationQuit() {
        OnlineLogging.Instance.Close();
    }

    #endregion

    #region Public Methods

    public void SwitchToAskUsernamePanel () {
        SwitchPanels (askUsernamePanel);
    }

    public void SwitchToLobbyMainMenu () {
        SwitchPanels (lobbyMenuPanel);
    }

    public void SwitchToCreateRoomPanel () {
        SwitchPanels (createRoomPanel);
    }

    public void SwitchToJoinPrivateRoomPanel() {
        SwitchPanels(joinPrivateRoomPanel);
    }

    public void SwitchToRoomPanel () {
        SwitchPanels (roomPanel);
    }

    #endregion

    #region Private Methods

    private void SwitchPanels (GameObject newPanel) {
        activePanel.SetActive (false);
        newPanel.SetActive (true);
        activePanel = newPanel;
    }

    #endregion

    #region Photon Callbacks

    public override void OnDisconnected (DisconnectCause cause) {
        string log = string.Format("Player disconnected. Cause: {0}", cause);
        OnlineLogging.Instance.Write(log);

        SwitchToAskUsernamePanel ();
    }

    public override void OnLeftRoom () {
        SwitchPanels (lobbyMenuPanel);
    }

    #endregion

    #region Properties

    public static OnlineLobbyManager Instance {
        get => instance;
    }

    #endregion

}
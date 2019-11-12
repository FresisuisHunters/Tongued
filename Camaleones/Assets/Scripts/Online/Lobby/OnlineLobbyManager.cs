using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

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
        Debug.Log ("Player disconnected");
        Debug.LogWarning (cause);

        SwitchToAskUsernamePanel ();
    }

    public override void OnLeftRoom () {
        Debug.Log ("Player left room");

        SwitchPanels (lobbyMenuPanel);
    }

    #endregion

    #region Properties

    public static OnlineLobbyManager Instance {
        get => instance;
    }

    #endregion

}
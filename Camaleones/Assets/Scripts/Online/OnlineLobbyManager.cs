using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class OnlineLobbyManager : MonoBehaviourPunCallbacks {

    #region Constant Fields

    private const string CONNECTION_STATUS_MESSAGE = "Connection status:\n";

    #endregion

    #region Public Fields

    public TextMeshProUGUI connectionStatusText;

    // Ask Username Panel
    public GameObject askUsernamePanel;
    public TMP_InputField usernameInputField;

    // Lobby Menu Panel
    public GameObject lobbyMenuPanel;

    // Create Room Panel
    public GameObject createRoomPanel;
    public TMP_InputField roomNameInputField;
    public TMP_Dropdown roomSizeDropdown;

    // List Room Panel
    public GameObject listRoomPanel;
    public GameObject listEntriesParent;
    public GameObject listRoomEntry;
    public Dictionary<string, GameObject> roomEntries = new Dictionary<string, GameObject> ();

    // Room Panel
    public GameObject roomPanel;
    public TextMeshProUGUI playerListText;
    public GameObject startGameButton;

    #endregion

    #region Private Fields

    private static OnlineLobbyManager instance;
    private GameObject activePanel;
    private bool listRooms = false;

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

    public void JoinRoom (string roomName) {
        PhotonNetwork.JoinRoom (roomName);
    }

    #endregion

    #region Private Methods

    private void ConnectToPhotonServer () {
        if (PhotonNetwork.IsConnected) {
            Debug.LogError ("Player already connected to Photon server");
            return;
        }

        PhotonNetwork.GameVersion = ServerConstants.GAME_VERSION;
        PhotonNetwork.ConnectUsingSettings ();
    }

    private void UpdatePlayersList () {
        Player[] playersInRoom = PhotonNetwork.PlayerList;

        playerListText.text = "";
        for (int i = 0; i < playersInRoom.Length; ++i) {
            Player player = playersInRoom[i];

            playerListText.text += player.ToString () + "\n";
        }
    }

    private void SwitchPanels (GameObject newPanel) {
        activePanel.SetActive (false);
        newPanel.SetActive (true);
        activePanel = newPanel;
    }

    #endregion

    #region Photon Callbacks

    public override void OnConnectedToMaster () {
        Debug.Log ("OnConnectedToMaster");
        SwitchPanels (lobbyMenuPanel);
    }

    public override void OnJoinedLobby () {
        Debug.Log ("OnJoinedLobby");

    }

    public override void OnDisconnected (DisconnectCause cause) {
        Debug.Log ("OnDisconnected");
        Debug.Log (cause);

        SwitchPanels (askUsernamePanel);
    }

    public override void OnLeftLobby () {
        Debug.Log ("OnLeftLobby");
    }

    public override void OnCreatedRoom () {
        Debug.Log ("OnCreatedRoom");

        SwitchPanels (roomPanel);
        startGameButton.SetActive (PhotonNetwork.LocalPlayer.IsMasterClient);

        UpdatePlayersList ();
    }

    public override void OnCreateRoomFailed (short returnCode, string message) {
        Debug.Log ("OnCreateRoomFailed");
        Debug.LogError (message);
    }

    public override void OnJoinRoomFailed (short returnCode, string message) {
        Debug.Log ("OnJoinRoomFailed");
        Debug.LogError (message);
    }

    public override void OnJoinRandomFailed (short returnCode, string message) {
        Debug.Log ("OnJoinRandomRoomFailed");
        Debug.LogError (message);
    }

    public override void OnJoinedRoom () {
        Debug.Log ("OnJoinedRoom");

        SwitchPanels (roomPanel);
        startGameButton.SetActive (PhotonNetwork.LocalPlayer.IsMasterClient);

        UpdatePlayersList ();
    }

    public override void OnLeftRoom () {
        Debug.Log ("OnLeftRoom");

        SwitchPanels (lobbyMenuPanel);
    }

    public override void OnPlayerEnteredRoom (Player newPlayer) {
        Debug.Log ("OnPlayerEnteredRoom");
        Debug.Log (newPlayer.NickName);

        UpdatePlayersList ();
    }

    public override void OnPlayerLeftRoom (Player otherPlayer) {
        Debug.Log ("OnPlayerLeftRoom");
        Debug.Log (otherPlayer.NickName);

        startGameButton.SetActive (PhotonNetwork.LocalPlayer.IsMasterClient);
        UpdatePlayersList();
    }

    public override void OnRoomListUpdate (List<RoomInfo> roomList) {
        Debug.Log ("OnRoomListUpdate");

        if (!listRooms) {
            return;
        }

        // TODO: Mejorar
        foreach (GameObject listEntry in roomEntries.Values) {
            GameObject.Destroy (listEntry);
        }
        roomEntries.Clear ();

        for (int i = 0; i < roomList.Count; ++i) {
            RoomInfo roomInfo = roomList[i];

            float x = listRoomEntry.transform.position.x;
            float y = listRoomEntry.transform.position.y + i * 21; // TODO: Sacar la altura del rect transform
            Vector2 position = new Vector2 (x, y);
            Quaternion rotation = Quaternion.identity;
            Transform parent = listEntriesParent.transform;
            GameObject roomEntryGameObject = GameObject.Instantiate (listRoomEntry, position, rotation, parent);
            roomEntryGameObject.GetComponent<RoomListEntry> ().SetRoomValues (roomInfo);

            string roomName = roomInfo.Name;
            roomEntries.Add (roomName, roomEntryGameObject);
        }
    }

    #endregion

    #region UI Callbacks

    #region Ask username panel callbacks

    public void OnConnectToServerButtonClicked () {
        PhotonNetwork.LocalPlayer.NickName = usernameInputField.text;

        ConnectToPhotonServer ();
    }

    public void OnGoToMainMenuButtonClicked () {
        Debug.LogWarning ("TODO: Volver al menú principal");
    }

    #endregion

    #region Lobby menu panel callbacks

    public void OnCreateRoomButtonClicked () {
        SwitchPanels (createRoomPanel);
    }

    public void OnJoinRandomRoomButtonClicked () {
        PhotonNetwork.JoinRandomRoom ();
    }

    public void OnListRoomsButtonClicked () {
        SwitchPanels (listRoomPanel);

        listRooms = true;
        PhotonNetwork.JoinLobby ();
    }

    public void OnQuitLobbyButtonClicked () {
        PhotonNetwork.Disconnect ();
    }

    #endregion

    #region Create room panel callbacks

    public void OnCreateButtonClicked () {
        string roomName = roomNameInputField.text;

        int dropdownSelectedItemIndex = roomSizeDropdown.value;
        string roomSizeString = roomSizeDropdown.options[dropdownSelectedItemIndex].text;
        byte roomSize = byte.Parse (roomSizeString);

        RoomOptions roomOptions = new RoomOptions ();
        roomOptions.MaxPlayers = roomSize;
        PhotonNetwork.CreateRoom (roomName, roomOptions);
    }

    public void OnQuitRoomCreationButtonClicked () {
        SwitchPanels (lobbyMenuPanel);
    }

    #endregion

    #region List room panel callbacks

    public void OnQuitRoomListingButtonClicked () {
        listRooms = false;
        foreach (GameObject listRoomEntry in roomEntries.Values) {
            GameObject.Destroy (listRoomEntry);
        }
        roomEntries.Clear ();
        SwitchPanels (lobbyMenuPanel);
    }

    #endregion

    #region Room panel callbacks

    public void OnStartGameButtonClicked () {
        PhotonNetwork.LoadLevel (ServerConstants.ONLINE_LEVEL);
    }

    // TODO: Continuar aqui
    public void OnQuitRoomButtonClicked () {
        PhotonNetwork.LeaveRoom ();
    }

    #endregion

    #endregion

    #region Properties

    public static OnlineLobbyManager Instance {
        get => instance;
    }

    #endregion

}
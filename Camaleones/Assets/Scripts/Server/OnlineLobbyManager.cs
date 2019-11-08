using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    // Room Panel
    public GameObject roomPanel;
    public TextMeshProUGUI playerListText;

    #endregion

    #region Private Fields

    [SerializeField] private string playerUsername;
    private GameObject activePanel;

    #endregion

    #region Unity Callbacks

    private void Awake () {
        PhotonNetwork.AutomaticallySyncScene = true;

        activePanel = askUsernamePanel;
    }

    private void Update () {
        connectionStatusText.text = CONNECTION_STATUS_MESSAGE + PhotonNetwork.NetworkClientState;
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

    public override void OnLeftLobby () {
        Debug.Log ("OnLeftLobby");
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
        UpdatePlayersList ();
    }

    public override void OnLeftRoom () {
        Debug.Log ("OnLeftRoom");
    }

    public override void OnPlayerEnteredRoom (Player newPlayer) {
        Debug.Log ("OnPlayerEnteredRoom");
        Debug.Log (newPlayer.NickName);

        UpdatePlayersList ();
    }

    public override void OnPlayerLeftRoom (Player otherPlayer) {
        Debug.Log ("OnPlayerLeftRoom");
        Debug.Log (otherPlayer.NickName);
    }

    #endregion

    #region UI Callbacks

    public void OnConnectToServerButtonPressed () {
        // TODO: Crear un Player de Photon con esta informacion
        playerUsername = usernameInputField.text;

        ConnectToPhotonServer ();
    }

    public void OnCreateRoomButtonClicked () {
        SwitchPanels (createRoomPanel);
    }

    public void OnJoinRandomRoomButtonClicked () {
        PhotonNetwork.JoinRandomRoom ();
    }

    public void OnListRoomsButtonClicked () {
        Debug.LogWarning ("Queda implementar listar las salas existentes");
    }

    public void OnCreateButtonClicked () {
        string roomName = roomNameInputField.text;

        int dropdownSelectedItemIndex = roomSizeDropdown.value;
        string roomSizeString = roomSizeDropdown.options[dropdownSelectedItemIndex].text;
        byte roomSize = byte.Parse (roomSizeString);

        RoomOptions roomOptions = new RoomOptions ();
        roomOptions.MaxPlayers = roomSize;
        PhotonNetwork.CreateRoom (roomName, roomOptions);
    }

    #endregion

}
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OnlineLobbyManager : MonoBehaviourPunCallbacks {

    private const string CONNECTION_STATUS_MESSAGE = "Connection status:\n";

    public TextMeshProUGUI connectionStatusText;
    public TMP_InputField usernameInputField;
    public GameObject buttonsPanel;
    public Button createRoomButton;
    public Button joinRandomRoomButton;
    public Button listRoomsButton;

    private void Awake () {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start () {
        ConnectToPhotonServer ();
    }

    private void ConnectToPhotonServer () {
        if (PhotonNetwork.IsConnected) {
            Debug.LogError ("Player already connected to Photon server");
            return;
        }

        PhotonNetwork.GameVersion = ServerConstants.GAME_VERSION;
        PhotonNetwork.ConnectUsingSettings ();
    }

    private void Update () {
        connectionStatusText.text = CONNECTION_STATUS_MESSAGE + PhotonNetwork.NetworkClientState;
    }

    #region Photon Callbacks

    public override void OnConnectedToMaster () {
        Debug.Log ("OnConnectedToMaster");
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
    }

    public override void OnLeftRoom () {
        Debug.Log ("OnLeftRoom");
    }

    public override void OnPlayerEnteredRoom (Player newPlayer) {
        Debug.Log ("OnPlayerEnteredRoom");
        Debug.Log (newPlayer.NickName);
    }

    public override void OnPlayerLeftRoom (Player otherPlayer) {
        Debug.Log ("OnPlayerLeftRoom");
        Debug.Log (otherPlayer.NickName);
    }

    #endregion

    #region UI Callbacks

    public void OnCreateRoomButtonClicked () {

    }

    public void OnJoinRandomRoomButtonClicked () {

    }

    public void OnListRoomsButtonClicked () {

    }

    #endregion

}
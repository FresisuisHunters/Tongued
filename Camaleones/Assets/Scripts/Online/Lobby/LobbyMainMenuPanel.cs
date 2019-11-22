using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Realtime;
using System.Linq;

#pragma warning disable 649
public class LobbyMainMenuPanel : MonoBehaviourPunCallbacks {

    #region Private Fields

    [SerializeField] private Button joinPublicGameButton;
    [SerializeField] private TMP_Dropdown gameModeDropDown;
    [SerializeField] private Button createPrivateRoomButton;
    [SerializeField] private Button joinPrivateRoomButton;
    [SerializeField] private Button quitLobbyMenuButton;
    private byte currentTries;

    #endregion

    #region Unity Callbacks

    private void Awake () {
        gameModeDropDown.AddOptions(new List<string>(ServerConstants.GAME_MODES));

        joinPublicGameButton.onClick.AddListener (() => OnJoinPublicGameButtonClicked ());
        createPrivateRoomButton.onClick.AddListener (() => OnCreatePrivateRoomButtonClicked ());
        joinPrivateRoomButton.onClick.AddListener (() => OnJoinPrivateRoomButtonClicked ());
        quitLobbyMenuButton.onClick.AddListener (() => OnQuitLobbyMenuButtonClicked ());
    }

    #endregion

    #region Photon Callbacks

    public override void OnJoinedLobby() {
        JoinOrCreateRoom();
    }

    public override void OnJoinedRoom() {
        OnlineLobbyManager.Instance.SwitchToRoomPanel();

        OnlineLogging.Instance.Write(PhotonNetwork.CurrentRoom.ToStringFull());
    }

    public override void OnJoinRandomFailed(short returnCode, string message) {
        ++currentTries;
        string log = string.Format("Failed to join public room. Try number {0}. Return code: {1}. Message: {2}", currentTries, returnCode, message);
        OnlineLogging.Instance.Write(log);

        Invoke("OnJoinPublicGameButtonClicked", ServerConstants.JOIN_RANDOM_RETRY_TIME);
    }

    #endregion

    #region Private Methods 

    private void JoinOrCreateRoom() {
        int gameModeIndexSelected = gameModeDropDown.value;
        string gameMode = gameModeDropDown.options[gameModeIndexSelected].text;

        Hashtable roomProperties = new Hashtable();
        roomProperties.Add(ServerConstants.GAME_MODE_ROOM_KEY, gameMode);

        if (currentTries < ServerConstants.JOIN_RANDOM_ROOM_TRIES) {
            PhotonNetwork.JoinRandomRoom(roomProperties, 0, MatchmakingMode.FillRoom, TypedLobby.Default, "");
        } else {
            string roomName = RandomString(10);
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 4;
            roomOptions.CustomRoomProperties = roomProperties;
            roomOptions.CustomRoomPropertiesForLobby = new string[] { gameMode };

            PhotonNetwork.CreateRoom(roomName, roomOptions, TypedLobby.Default);
        }
    }

    private static string RandomString(int length) {
        System.Random random = new System.Random();  
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    #endregion

    #region UI Callbacks

    private void OnJoinPublicGameButtonClicked () {
        int gameModeIndexSelected = gameModeDropDown.value;
        string gameMode = gameModeDropDown.options[gameModeIndexSelected].text;
        TypedLobby lobby = ServerConstants.GAME_MODES_LOBBIES[gameMode];
        PhotonNetwork.JoinLobby(lobby);
    }

    private void OnCreatePrivateRoomButtonClicked () {
        OnlineLobbyManager.Instance.SwitchToCreateRoomPanel ();
    }

    private void OnJoinPrivateRoomButtonClicked () {
        OnlineLobbyManager.Instance.SwitchToJoinPrivateRoomPanel();
    }

    private void OnQuitLobbyMenuButtonClicked () {
        PhotonNetwork.Disconnect();
    }

    #endregion

}
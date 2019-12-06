using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

#pragma warning disable 649
public class LobbyScreen : AMenuScreen, ILobbyCallbacks, IMatchmakingCallbacks
{
    #region Inspector
    [SerializeField] private TMPro.TMP_InputField roomNameInput;
    [SerializeField] private TMPro.TextMeshProUGUI messageField;
    [SerializeField] private Button joinPublicRoomButton;
    [SerializeField] private Button cancelPublicSearchButton;
    [SerializeField] private Button backButton;
    #endregion

    private byte currentAtempts;

    #region Screen Operations
    protected override void OnOpen(Type previousScreen)
    {
        PhotonNetwork.AddCallbackTarget(this);
        currentAtempts = 0;

        joinPublicRoomButton.gameObject.SetActive(true);
        cancelPublicSearchButton.gameObject.SetActive(false);

        messageField.text = string.Format("Welcome to the lobby, {0}!", PhotonNetwork.LocalPlayer.NickName);
    }

    protected override void OnClose(Type nextScreen) => PhotonNetwork.RemoveCallbackTarget(this);

    public override void GoBack()
    {
        PhotonNetwork.Disconnect();
        MenuManager.SetActiveMenuScreen<AskUsernameScreen>();
    }
    #endregion

    #region Buttons
    public void JoinPublicRoom()
    {
        TypedLobby lobby = ServerConstants.GAME_MODE_1_LOBBY;
        PhotonNetwork.JoinLobby(lobby);

        joinPublicRoomButton.gameObject.SetActive(false);
        cancelPublicSearchButton.gameObject.SetActive(true);

        SetInteractable(false, cancelPublicSearchButton, backButton);

        messageField.text = "Searching...";
    }

    public void CancelPublicSearch()
    {
        if (PhotonNetwork.InLobby) PhotonNetwork.LeaveLobby();
        CancelInvoke();

        joinPublicRoomButton.gameObject.SetActive(true);
        cancelPublicSearchButton.gameObject.SetActive(false);

        SetInteractable(true);

        messageField.text = "";
    }

    public void CreatePrivateRoom()
    {
        string roomName = roomNameInput.text;
        print(roomName);
        print(roomNameInput.textComponent.text);

        if (string.IsNullOrEmpty(roomName) || string.IsNullOrWhiteSpace(roomName))
        {
            return;
        }

        SetInteractable(false);

        byte roomSize = 4;
        string gameModeString = ServerConstants.GAME_MODE_1;

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = roomSize;
        roomOptions.IsVisible = false;
        roomOptions.CustomRoomProperties = new Hashtable();
        roomOptions.CustomRoomProperties.Add(ServerConstants.GAME_MODE_ROOM_KEY, gameModeString);

        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    public void JoinPrivateRoom()
    {
        string roomName = roomNameInput.text;
        if (string.IsNullOrEmpty(roomName) || string.IsNullOrWhiteSpace(roomName))
        {
            return;
        }

        SetInteractable(false);
        PhotonNetwork.JoinRoom(roomName);
    }
    #endregion

    private void TryToJoinPublicRoom()
    {
        Hashtable roomProperties = new Hashtable();
        roomProperties.Add(ServerConstants.GAME_MODE_ROOM_KEY, ServerConstants.GAME_MODE_1);

        if (currentAtempts < ServerConstants.JOIN_RANDOM_ROOM_TRIES)
        {
            PhotonNetwork.JoinRandomRoom(roomProperties, 0, MatchmakingMode.FillRoom, TypedLobby.Default, "");
        }
        else
        {
            string roomName = RandomString(10);
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 4;
            roomOptions.CustomRoomProperties = roomProperties;
            roomOptions.CustomRoomPropertiesForLobby = new string[] { ServerConstants.GAME_MODE_1 };

            PhotonNetwork.CreateRoom(roomName, roomOptions, TypedLobby.Default);
        }
    }

    private void GoToRoom()
    {
        OnlineLogging.Instance.Write(PhotonNetwork.CurrentRoom.ToStringFull());

        MenuManager.SetActiveMenuScreen<RoomScreen>();
    }

    #region Lobby Callbacks
    void ILobbyCallbacks.OnJoinedLobby() => TryToJoinPublicRoom();

    void ILobbyCallbacks.OnLeftLobby() { }
    void ILobbyCallbacks.OnRoomListUpdate(List<RoomInfo> roomList) { }
    void ILobbyCallbacks.OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics) { }
    #endregion

    #region Matchmaking Callbacks
    void IMatchmakingCallbacks.OnFriendListUpdate(List<FriendInfo> friendList) { }
    void IMatchmakingCallbacks.OnCreatedRoom() { }

    void IMatchmakingCallbacks.OnCreateRoomFailed(short returnCode, string message)
    {
        string log = string.Format("Error creating room\nReturn code: {0}\nError message: {1}", returnCode, message);
        OnlineLogging.Instance.Write(log);

        SetInteractable(true);

        if (returnCode == ErrorCode.GameIdAlreadyExists) message = "That room name is already taken.";
        messageField.text = message;
    }

    void IMatchmakingCallbacks.OnJoinedRoom() => GoToRoom();

    void IMatchmakingCallbacks.OnJoinRoomFailed(short returnCode, string message)
    {
        string log = string.Format("Couldn't join room. Return code: {0}. Error message: {1}", returnCode, message);
        OnlineLogging.Instance.Write(log);

        SetInteractable(true);

        if (returnCode == ErrorCode.GameFull) message = "This room is already full.";
        else if (returnCode == ErrorCode.GameClosed) message = "This room is closed.";
        else if (returnCode == ErrorCode.GameDoesNotExist) message = "There is no room with that name.";
        else if (returnCode == ErrorCode.JoinFailedFoundActiveJoiner) message = "There is a player with your same username in this room";
        messageField.text = message;
    }

    void IMatchmakingCallbacks.OnJoinRandomFailed(short returnCode, string message)
    {
        currentAtempts++;

        string log = string.Format("Failed to join public room. Try number {0}. Return code: {1}. Message: {2}", currentAtempts, returnCode, message);
        OnlineLogging.Instance.Write(log);

        Invoke("TryToJoinPublicRoom", ServerConstants.JOIN_RANDOM_RETRY_TIME);
    }

    void IMatchmakingCallbacks.OnLeftRoom() { }
    #endregion

    private static string RandomString(int length)
    {
        System.Random random = new System.Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

}

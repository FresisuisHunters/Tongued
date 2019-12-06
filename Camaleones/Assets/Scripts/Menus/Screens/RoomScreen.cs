using System;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections.Generic;
using UnityEngine.UI;

#pragma warning disable 649
[RequireComponent(typeof(PhotonView))]
public class RoomScreen : AMenuScreen, IMatchmakingCallbacks, IInRoomCallbacks
{
    #region Inspector
    [SerializeField] private TextMeshProUGUI roomNameField;

    [Header("Countdown")]
    [SerializeField] private float countdownLength;
    [SerializeField] private TextMeshProUGUI countdownTextField;
    [SerializeField] private TextMeshProUGUI playerCountTextField;

    [Header("Player table")]
    [SerializeField] private RoomPlayerEntry playerEntryPrefab;
    [SerializeField] private RectTransform playerTableParent;

    [Header("Ready button")]
    [SerializeField] private Button readyButton;
    [SerializeField] private Button notReadyButton;

    [Header("Game setup prefabs")]
    [SerializeField] OnlineHotPotatoHandler onlineHotPotatoHandlerPrefab;
    [SerializeField] OnlineGameScreenCallbackHandler onlineGameCallbackHandlerPrefab;

    #endregion

    #region Private State
    public bool LocalPlayerIsReady { get; private set; }

    private bool isDoingCountdown;
    private float currentCountdownValue;

    private Dictionary<string, RoomPlayerEntry> players = new Dictionary<string, RoomPlayerEntry>();
    private List<string> playersReady = new List<string>();

    private PhotonView photonView;
    #endregion

    #region Screen Operations
    protected override void OnOpen(Type previousScreen)
    {
        PhotonNetwork.AddCallbackTarget(this);
        PhotonNetwork.AutomaticallySyncScene = true;

        ClearPlayerEntryState();

        foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            CreatePlayerEntry(player);
        }

        isDoingCountdown = false;

        countdownTextField.gameObject.SetActive(false);
        SetLocalPlayerReady(false);

        if (!PhotonNetwork.CurrentRoom.IsVisible) roomNameField.text = "ROOM: " + PhotonNetwork.CurrentRoom.Name;
        else roomNameField.text = "";

        UpdateRoomCapacityText();
        CheckIfShouldStartCountdown();
    }

    protected override void OnClose(Type nextScreen)
    {
        PhotonNetwork.RemoveCallbackTarget(this);
        ClearPlayerEntryState();
    }

    public override void GoBack()
    {
        PhotonNetwork.LeaveRoom();
    }
    #endregion

    #region Countdown & Game Start
    public void SetLocalPlayerReady(bool isReady)
    {
        if (isReady) photonView.RPC("RPC_PlayerIsReady", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.NickName);
        else photonView.RPC("RPC_PlayerNotReady", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.NickName);

        readyButton.gameObject.SetActive(!isReady);
        notReadyButton.gameObject.SetActive(isReady);
    }

    private void Update()
    {
        if (isDoingCountdown && currentCountdownValue > 0)
        {
            currentCountdownValue -= Time.deltaTime;
            currentCountdownValue = Mathf.Max(currentCountdownValue, 0);
            countdownTextField.text = Mathf.CeilToInt(currentCountdownValue).ToString();

            if (currentCountdownValue <= 0f) StartGame();
        }
    }

    [PunRPC]
    private void StartCountdown()
    {
        isDoingCountdown = true;
        currentCountdownValue = countdownLength;
        countdownTextField.gameObject.SetActive(true);
    }

    private void StartGame()
    {
        SetInteractable(false);

        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            SceneManagerExtensions.PhotonLoadScene(ServerConstants.ONLINE_LEVEL, () =>
            {
                PhotonNetwork.InstantiateSceneObject(onlineHotPotatoHandlerPrefab.name, Vector3.zero, Quaternion.identity);
                PhotonNetwork.RemoveCallbackTarget(this);

                Instantiate(onlineGameCallbackHandlerPrefab);
            });
        }
    }

    [PunRPC]
    private void StopCountdown()
    {
        countdownTextField.gameObject.SetActive(false);
        isDoingCountdown = false;
    }

    [PunRPC]
    public void RPC_PlayerIsReady(string playerName)
    {
        playersReady.Add(playerName);
        players[playerName].IsReady = true;
        CheckIfShouldStartCountdown();
    }

    [PunRPC]
    public void RPC_PlayerNotReady(string playerName)
    {
        photonView.RPC("StopCountdown", RpcTarget.All, null);

        playersReady.Remove(playerName);
        players[playerName].IsReady = false;
        CheckIfShouldStartCountdown();
    }
    #endregion

    #region UI Operations
    private void ClearPlayerEntryState()
    {
        foreach (RoomPlayerEntry entry in players.Values)
        {
            Destroy(entry.gameObject);
        }

        players.Clear();
        playersReady.Clear();
    }

    private void CreatePlayerEntry(Player newPlayer)
    {
        string playerName = newPlayer.NickName;
        RoomPlayerEntry playerEntry = Instantiate(playerEntryPrefab, playerTableParent, false);

        players.Add(playerName, playerEntry);
        playerEntry.PlayerName = playerName;
        playerEntry.IsReady = false;
    }

    private void RemovePlayerEntry(Player otherPlayer)
    {
        string playerName = otherPlayer.NickName;
        RoomPlayerEntry entry = players[playerName];
        players.Remove(playerName);
        playersReady.Remove(playerName);

        Destroy(entry.gameObject);
    }

    private void UpdateRoomCapacityText()
    {
        byte currentPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
        byte maxPlayers = PhotonNetwork.CurrentRoom.MaxPlayers;

        playerCountTextField.text = string.Format("{0}/{1}", currentPlayers, maxPlayers);
    }

    private void CheckIfShouldStartCountdown()
    {
        bool localIsRoomOwner = PhotonNetwork.LocalPlayer.IsMasterClient;
        bool allPlayersAreReady = true;// playersReady.Count == players.Count; TODO
        bool thereAreEnoughPlayers = playersReady.Count > 1;

        if (localIsRoomOwner && allPlayersAreReady && thereAreEnoughPlayers) photonView.RPC("StartCountdown", RpcTarget.All, null);
    }
    #endregion

    #region Matchmaking Callbacks
    void IMatchmakingCallbacks.OnLeftRoom() => MenuManager.SetActiveMenuScreen<LobbyScreen>();

    void IMatchmakingCallbacks.OnFriendListUpdate(List<FriendInfo> friendList) { }
    void IMatchmakingCallbacks.OnCreatedRoom() { }
    void IMatchmakingCallbacks.OnCreateRoomFailed(short returnCode, string message) { }
    void IMatchmakingCallbacks.OnJoinedRoom() { }
    void IMatchmakingCallbacks.OnJoinRoomFailed(short returnCode, string message) { }
    void IMatchmakingCallbacks.OnJoinRandomFailed(short returnCode, string message) { }
    #endregion

    #region In Room Callbacks
    void IInRoomCallbacks.OnPlayerEnteredRoom(Player newPlayer)
    {
        OnlineLogging.Instance.Write("OnPlayerEnteredRoom");

        CreatePlayerEntry(newPlayer);
        UpdateRoomCapacityText();
        CheckIfShouldStartCountdown();
    }

    void IInRoomCallbacks.OnPlayerLeftRoom(Player otherPlayer)
    {
        RemovePlayerEntry(otherPlayer);
        UpdateRoomCapacityText();
        CheckIfShouldStartCountdown();
    }

    void IInRoomCallbacks.OnRoomPropertiesUpdate(Hashtable propertiesThatChanged) { }
    void IInRoomCallbacks.OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps) { }
    void IInRoomCallbacks.OnMasterClientSwitched(Player newMasterClient) { }
    #endregion

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }
}

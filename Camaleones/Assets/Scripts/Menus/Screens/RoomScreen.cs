using System;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections.Generic;

[RequireComponent(typeof(PhotonView))]
public class RoomScreen : AMenuScreen, IMatchmakingCallbacks, IInRoomCallbacks
{
    private const string HOT_POTATO_HANDLER_PREFAB_NAME = "Online Hot Potato Manager";

    #region Inspector
    [SerializeField] private float countdownLength;
    [SerializeField] private TextMeshProUGUI countdownTextField;
    [SerializeField] private TextMeshProUGUI playerCountTextField;
    [SerializeField] private GameObject playerEntryPrefab;
    #endregion

    #region Private State
    private bool isDoingCountdown;
    private float currentCountdownValue;

    private Dictionary<string, RoomPlayerEntry> players = new Dictionary<string, RoomPlayerEntry>();
    private List<string> playersReady = new List<string>();
    private Stack<RoomPlayerEntry> unusedPlayerEntries = new Stack<RoomPlayerEntry>();

    public PhotonView photonView { get; private set; }

    #endregion

    #region Screen Operations
    protected override void OnOpen(Type previousScreen)
    {
        PhotonNetwork.AddCallbackTarget(this);
        PhotonNetwork.AutomaticallySyncScene = true;

        isDoingCountdown = false;
        UpdateRoomCapacityText();

        foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            CreatePlayerEntry(player);
        }

        CheckIfShouldStartCountdown();
        UpdatePlayersList();

        countdownTextField.gameObject.SetActive(false);
    }

    protected override void OnClose(Type nextScreen)
    {
        PhotonNetwork.RemoveCallbackTarget(this);
        players.Clear();
        playersReady.Clear();
    }

    public override void GoBack()
    {
        PhotonNetwork.LeaveRoom();
    }
    #endregion

    #region Countdown & Game Start
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
            OnlineLogging.Instance.Write("El jugador " + PhotonNetwork.LocalPlayer.NickName + " inicia la partida.");

            SceneManagerExtensions.PhotonLoadScene(ServerConstants.ONLINE_LEVEL, () =>
            {
                PhotonNetwork.InstantiateSceneObject(HOT_POTATO_HANDLER_PREFAB_NAME, Vector3.zero, Quaternion.identity);
                PhotonNetwork.RemoveCallbackTarget(this);
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
        players[playerName].Text = string.Format("* {0}", playerName);
        CheckIfShouldStartCountdown();
    }

    [PunRPC]
    public void RPC_PlayerNotReady(string playerName)
    {
        photonView.RPC("StopCountdown", RpcTarget.All, null);
        players[playerName].Text = playerName;
        playersReady.Remove(playerName);
        CheckIfShouldStartCountdown();
    }


    #endregion

    #region UI Operations
    private void CreatePlayerEntry(Player newPlayer)
    {
        Debug.Log("Creating entry");
        string playerName = newPlayer.NickName;
        RoomPlayerEntry playerEntry = GetPlayerEntry();

        players.Add(playerName, playerEntry);
        playerEntry.PlayerName = playerName;
        playerEntry.Room = this;
        playerEntry.Visible = true;
    }

    private RoomPlayerEntry GetPlayerEntry()
    {
        if (unusedPlayerEntries.Count != 0)
        {
            RoomPlayerEntry entry = unusedPlayerEntries.Pop();
            return entry;
        }

        GameObject newEntry = GameObject.Instantiate(playerEntryPrefab, Vector3.zero, Quaternion.identity);
        newEntry.transform.SetParent(transform, false);

        return newEntry.GetComponent<RoomPlayerEntry>();
    }

    private void RemovePlayerEntry(Player otherPlayer)
    {
        string playerName = otherPlayer.NickName;
        RoomPlayerEntry entry = players[playerName];
        players.Remove(playerName);
        playersReady.Remove(playerName);

        entry.Visible = false;
        unusedPlayerEntries.Push(entry);
    }

    private void UpdateRoomCapacityText()
    {
        byte currentPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
        byte maxPlayers = PhotonNetwork.CurrentRoom.MaxPlayers;

        playerCountTextField.text = string.Format("{0}/{1}", currentPlayers, maxPlayers);

        //roomNameText.text = PhotonNetwork.CurrentRoom.Name;
    }

    private void UpdatePlayersList()
    {
        Vector3 firstEntryPosition = Vector3.zero;

        int i = 0;
        foreach (RoomPlayerEntry playerEntry in players.Values)
        {
            float y = i * -20f; // TODO: Declarar offset como constante
            ++i;

            Vector3 position = new Vector3(firstEntryPosition.x, y, 0f);
            playerEntry.Position = position;

            bool isLocalPlayer = playerEntry.PlayerName.Equals(PhotonNetwork.LocalPlayer.NickName);
            playerEntry.ShowButton(isLocalPlayer);
        }
    }

    private void CheckIfShouldStartCountdown()
    {
        bool localIsRoomOwner = PhotonNetwork.LocalPlayer.IsMasterClient;
        bool allPlayersAreReady = playersReady.Count == players.Count;

        if (localIsRoomOwner && allPlayersAreReady) photonView.RPC("StartCountdown", RpcTarget.All, null);
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
        UpdatePlayersList();
        CheckIfShouldStartCountdown();
    }

    void IInRoomCallbacks.OnPlayerLeftRoom(Player otherPlayer)
    {
        RemovePlayerEntry(otherPlayer);
        UpdateRoomCapacityText();
        UpdatePlayersList();
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

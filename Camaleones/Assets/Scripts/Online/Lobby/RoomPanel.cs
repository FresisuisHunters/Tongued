using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 649
public class RoomPanel : MonoBehaviourPunCallbacks {

    #region Constant Fields

    private const float GAME_COUNTDOWN = .5f;

    #endregion

    #region Private Fields

    [SerializeField] private GameObject playerEntryGameObject;
    [SerializeField] private TextMeshProUGUI gameModeText;
    [SerializeField] private TextMeshProUGUI roomSizeText;
    [SerializeField] private TextMeshProUGUI roomNameText;
    [SerializeField] private TextMeshProUGUI gameCountdownText;
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button quitRoomButton;

    private Dictionary<string, RoomPlayerEntry> players = new Dictionary<string, RoomPlayerEntry> ();
    private Stack<RoomPlayerEntry> unusedPlayerEntries = new Stack<RoomPlayerEntry> ();
    private List<string> playersReady = new List<string> ();
    private float currentCountdown = GAME_COUNTDOWN;
    private bool startingGame = false;

    #endregion

    #region Unity Callbacks

    private void Awake () {
        startGameButton.onClick.AddListener (() => OnStartGameButtonClicked ());
        quitRoomButton.onClick.AddListener (() => OnQuitRoomButtonClicked ());
    }

    private void Update () {
        if (startingGame) {
            currentCountdown -= Time.deltaTime;
            if (currentCountdown <= 0f) {
                currentCountdown = 0;
                startingGame = false;
                StartGame ();
            }

            gameCountdownText.text = string.Format ("{0}", currentCountdown);
        }
    }

    protected new void OnEnable () {
        base.OnEnable ();

        startingGame = false;
        UpdateGameModeText ();
        UpdateRoomCapacityText();
        foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values) {
            CreatePlayerEntry(player);
        }
        UpdateStartGameButton();
        UpdatePlayersList();
    }

    #endregion

    #region Photon Callbacks

    public override void OnLeftRoom () {
        players.Clear ();
        playersReady.Clear ();
    }

    public override void OnPlayerEnteredRoom (Player newPlayer) {
        OnlineLogging.Instance.Write ("OnPlayerEnteredRoom");

        CreatePlayerEntry (newPlayer);
        UpdateRoomCapacityText ();
        UpdatePlayersList ();
        UpdateStartGameButton ();
    }

    public override void OnPlayerLeftRoom (Player otherPlayer) {
        OnlineLogging.Instance.Write ("OnPlayerLeftRoom");

        RemovePlayerEntry (otherPlayer);
        UpdateRoomCapacityText ();
        UpdatePlayersList ();
        UpdateStartGameButton ();
    }

    #endregion

    #region Public Methods

    [PunRPC]
    public void PlayerIsReady (string playerName) {
        playersReady.Add (playerName);
        players[playerName].Text = string.Format("* {0}", playerName);
        UpdateStartGameButton ();
    }

    [PunRPC]
    public void PlayerNotReady (string playerName) {
        photonView.RPC("StopGameCountdown", RpcTarget.All, null);
        players[playerName].Text = playerName;
        playersReady.Remove (playerName);
        UpdateStartGameButton ();
    }

    #endregion

    #region Private Methods

    private void CreatePlayerEntry (Player newPlayer) {
        string playerName = newPlayer.NickName;
        RoomPlayerEntry playerEntry = GetPlayerEntry ();

        players.Add (playerName, playerEntry);
        playerEntry.PlayerName = playerName;
        playerEntry.Room = this;
        playerEntry.Visible = true;
    }

    private RoomPlayerEntry GetPlayerEntry () {
        if (unusedPlayerEntries.Count != 0) {
            RoomPlayerEntry entry = unusedPlayerEntries.Pop ();
            return entry;
        }

        GameObject newEntry = GameObject.Instantiate (playerEntryGameObject, Vector3.zero, Quaternion.identity);
        newEntry.transform.SetParent(transform, false);

        return newEntry.GetComponent<RoomPlayerEntry> ();
    }

    private void RemovePlayerEntry (Player otherPlayer) {
        string playerName = otherPlayer.NickName;
        RoomPlayerEntry entry = players[playerName];
        players.Remove (playerName);
        playersReady.Remove (playerName);

        entry.Visible = false;
        unusedPlayerEntries.Push (entry);
    }

    private void UpdateGameModeText () {
        string gameMode = (string) PhotonNetwork.CurrentRoom.CustomProperties[ServerConstants.GAME_MODE_ROOM_KEY];
        gameModeText.text = gameMode;
    }

    private void UpdateRoomCapacityText () {
        byte currentPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
        byte maxPlayers = PhotonNetwork.CurrentRoom.MaxPlayers;

        roomSizeText.text = string.Format ("{0}/{1}", currentPlayers, maxPlayers);

        roomNameText.text = PhotonNetwork.CurrentRoom.Name;
    }

    private void UpdatePlayersList () {
        Vector3 firstEntryPosition = Vector3.zero;

        int i = 0;
        foreach (RoomPlayerEntry playerEntry in players.Values) {
            float y = i * -20f; // TODO: Declarar offset como constante
            ++i;

            Vector3 position = new Vector3 (firstEntryPosition.x, y, 0f);
            playerEntry.Position = position;

            bool isLocalPlayer = playerEntry.PlayerName.Equals(PhotonNetwork.LocalPlayer.NickName);
            playerEntry.ShowButton(isLocalPlayer);
        }
    }

    private void UpdateStartGameButton () {
        bool localIsRoomOwner = PhotonNetwork.LocalPlayer.IsMasterClient;
        bool allPlayersAreReady = playersReady.Count == players.Count;

        startGameButton.gameObject.SetActive (localIsRoomOwner);
        startGameButton.interactable = allPlayersAreReady;
    }

    [PunRPC]
    private void StartGameCountdown () {
        currentCountdown = GAME_COUNTDOWN;
        gameCountdownText.gameObject.SetActive (true);
        startingGame = true;
    }

    [PunRPC]
    private void StopGameCountdown () {
        gameCountdownText.gameObject.SetActive (false);
        startingGame = false;
    }

    private void StartGame () {
        if (PhotonNetwork.LocalPlayer.IsMasterClient) {
            OnlineLogging.Instance.Write("El jugador " + PhotonNetwork.LocalPlayer.NickName + " inicial la partida");
            PhotonNetwork.LoadLevel (ServerConstants.ONLINE_LEVEL);
        }
    }

    #endregion

    #region UI Callbacks

    private void OnStartGameButtonClicked () {
        startGameButton.interactable = false;
        photonView.RPC("StartGameCountdown", RpcTarget.All, null);
    }

    private void OnQuitRoomButtonClicked () {
        PhotonNetwork.LeaveRoom ();
    }

    #endregion

}
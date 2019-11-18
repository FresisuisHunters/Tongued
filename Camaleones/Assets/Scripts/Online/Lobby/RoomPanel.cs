using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomPanel : MonoBehaviourPunCallbacks {

    #region Constant Fields

    private const float GAME_COUNTDOWN = 5f;

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
                StartGame ();
            }

            gameCountdownText.text = string.Format ("{0}", currentCountdown);
        }
    }

    protected new void OnEnable () {
        base.OnEnable ();

        startingGame = false;
        UpdateGameModeText ();
        OnPlayerEnteredRoom(PhotonNetwork.LocalPlayer);
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

    public void PlayerIsReady (string playerName) {
        playersReady.Add (playerName);
        UpdateStartGameButton ();
    }

    public void PlayerNotReady (string playerName) {
        StopGameCountdown();
        playersReady.Remove (playerName);
        UpdateStartGameButton ();
    }

    #endregion

    #region Private Methods

    private void CreatePlayerEntry (Player newPlayer) {
        string playerName = newPlayer.NickName;
        RoomPlayerEntry playerEntry = GetPlayerEntry ();

        players.Add(playerName, playerEntry);
        playerEntry.PlayerName = playerName;
        playerEntry.Room = this;
    }

    private RoomPlayerEntry GetPlayerEntry () {
        if (unusedPlayerEntries.Count != 0) {
            RoomPlayerEntry entry = unusedPlayerEntries.Pop ();
            entry.Visible = true;
            return entry;
        }

        GameObject newEntry = GameObject.Instantiate (playerEntryGameObject, Vector3.zero, Quaternion.identity, transform);
        return newEntry.GetComponent<RoomPlayerEntry>();
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
        // TODO: Sacar posicion de otro punto
        Vector3 firstEntryPosition = GetComponent<RectTransform>().position;
        // firstEntryPosition.x = firstEntryPosition.x / 2;

        Debug.Log(firstEntryPosition);
        Debug.Log(players.Count);

        int i = 0;
        foreach (RoomPlayerEntry playerEntry in players.Values) {
            float y = firstEntryPosition.y + i++ * 20f; // TODO: Declarar offset como constante
            Vector3 position = new Vector3 (firstEntryPosition.x, y, 0f);
            position.x += 200;
            position.y = y + 200;

            Debug.Log(position);

            playerEntry.Position = position;
        }
    }

    private void UpdateStartGameButton () {
        bool localIsRoomOwner = PhotonNetwork.LocalPlayer.IsMasterClient;
        bool allPlayersAreReady = playersReady.Count == players.Count;

        Debug.Log(allPlayersAreReady);

        startGameButton.gameObject.SetActive (localIsRoomOwner);
        startGameButton.interactable = allPlayersAreReady;
    }

    private void StartGameCountdown () {
        currentCountdown = GAME_COUNTDOWN;
        gameCountdownText.gameObject.SetActive (true);
        startingGame = true;
    }

    private void StopGameCountdown () {
        gameCountdownText.gameObject.SetActive (false);
        startingGame = false;
    }

    private void StartGame () {
        if (PhotonNetwork.LocalPlayer.IsMasterClient) {
            PhotonNetwork.LoadLevel (ServerConstants.ONLINE_LEVEL);
        }
    }

    #endregion

    #region UI Callbacks

    private void OnStartGameButtonClicked () {
        startGameButton.interactable = false;
        StartGameCountdown ();
    }

    private void OnQuitRoomButtonClicked () {
        PhotonNetwork.LeaveRoom ();
    }

    #endregion

}
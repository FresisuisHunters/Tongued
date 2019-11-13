using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomPanel : MonoBehaviourPunCallbacks {

    #region Private Fields

    [SerializeField] private TextMeshProUGUI gameModeText;
    [SerializeField] private TextMeshProUGUI roomSizeText;
    [SerializeField] private TextMeshProUGUI roomNameText;
    [SerializeField] private TextMeshProUGUI playersListText;
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button quitRoomButton;

    #endregion

    #region Unity Callbacks

    private void Awake () {
        startGameButton.onClick.AddListener (() => OnStartGameButtonClicked ());
        quitRoomButton.onClick.AddListener (() => OnQuitRoomButtonClicked ());
    }

    protected new void OnEnable () {
        UpdateGameModeText();
        UpdateRoomCapacityText ();
        UpdatePlayersText ();
        UpdateStartGameButton();
    }

    #endregion

    #region Photon Callbacks

    public override void OnPlayerEnteredRoom(Player newPlayer) {
        OnlineLogging.Instance.Write("OnPlayerEnteredRoom");

        UpdateRoomCapacityText();
        UpdatePlayersText();
        UpdateStartGameButton();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) {
        OnlineLogging.Instance.Write("OnPlayerLeftRoom");

        UpdateRoomCapacityText();
        UpdatePlayersText();
        UpdateStartGameButton();
    }

    #endregion

    #region Private Methods

    private void UpdateGameModeText() {
        string gameMode = (string) PhotonNetwork.CurrentRoom.CustomProperties[ServerConstants.GAME_MODE_ROOM_KEY];
        gameModeText.text = gameMode;
    }

    private void UpdateRoomCapacityText () {
        byte currentPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
        byte maxPlayers = PhotonNetwork.CurrentRoom.MaxPlayers;
        
        roomSizeText.text = string.Format ("{0}/{1}", currentPlayers, maxPlayers);

        roomNameText.text = PhotonNetwork.CurrentRoom.Name;
    }

    private void UpdatePlayersText () {
        playersListText.text = "";
        Player[] players = PhotonNetwork.PlayerList;
        for (int i = 0; i < players.Length; ++i) {
            Player p = players[i];
            playersListText.text += string.Format ("{0}\n", p.ToStringFull ());
        }
    }

    private void UpdateStartGameButton() {
        startGameButton.gameObject.SetActive(PhotonNetwork.LocalPlayer.IsMasterClient);
    }

    #endregion

    #region UI Callbacks

    private void OnStartGameButtonClicked () {
        PhotonNetwork.LoadLevel (ServerConstants.ONLINE_LEVEL);
    }

    private void OnQuitRoomButtonClicked () {
        PhotonNetwork.LeaveRoom ();
    }

    #endregion

}
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomPanel : MonoBehaviourPunCallbacks {

    #region Public Fields

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
        Debug.Log ("OnEnable");
        UpdateRoomCapacityText ();
        UpdatePlayersText ();
    }

    #endregion

    #region Private Methods

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
            playersListText.text += string.Format ("{0}\n" + p.ToStringFull ());
        }
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
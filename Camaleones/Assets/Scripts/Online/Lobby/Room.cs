using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class Room : MonoBehaviourPunCallbacks
{

    #region Public Fields

    public TextMeshProUGUI roomSizeText;
    public TextMeshProUGUI roomNameText;
    public TextMeshProUGUI playersListText;
    public Button startGameButton;
    public Button quitRoomButton;

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        startGameButton.onClick.AddListener(() => OnStartGameButtonClicked());
        quitRoomButton.onClick.AddListener(() => OnQuitRoomButtonClicked());
    }

    protected new void OnEnable() {
        Debug.Log("OnEnable");
        
        byte currentPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
        byte maxPlayers = PhotonNetwork.CurrentRoom.MaxPlayers;
        roomSizeText.text = string.Format("{0}/{1}", currentPlayers, maxPlayers);

        roomNameText.text = PhotonNetwork.CurrentRoom.Name;
    }

    #endregion

    #region UI Callbacks

    private void OnStartGameButtonClicked() {

    }

    private void OnQuitRoomButtonClicked() {

    }

    #endregion

}

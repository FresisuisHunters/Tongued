using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomListEntry : MonoBehaviour {

    #region Public Fields

    public TextMeshProUGUI roomName;
    public TextMeshProUGUI roomCapacity;
    public Button joinRoomButton;

    #endregion

    #region Unity Callbacks

    private void Start () {
        joinRoomButton.onClick.AddListener (() => {
            OnlineLobbyManager.Instance.JoinRoom (roomName.text);
        });
    }

    #endregion

    #region Public Methods

    public void SetRoomValues (RoomInfo room) {
        roomName.text = room.Name;
        roomCapacity.text = string.Format ("{0}/{1}", room.PlayerCount, room.MaxPlayers);
    }

    #endregion

    #region Private Methods

    #endregion

}
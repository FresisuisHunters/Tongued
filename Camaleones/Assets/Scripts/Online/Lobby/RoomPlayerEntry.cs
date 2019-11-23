using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class RoomPlayerEntry : MonoBehaviour {

    #region Private Fields

    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private Button playerReadyButton;
    private RectTransform rectTransform;
    private RoomPanel room;
    private string playerName;
    private bool buttonClicked;

    #endregion

    #region Unity Callbacks

    private void Awake () {
        rectTransform = GetComponent<RectTransform>();
        buttonClicked = false;
        playerReadyButton.onClick.AddListener (() => onPlayerReadyButtonClicked ());
    }

    #endregion

    #region Public Methods

    public void ShowButton(bool show) {
        playerReadyButton.gameObject.SetActive(show);
    }

    public override string ToString() {
        return string.Format("ROOM_PLAYER_ENTRY: {0}, {1}", playerName, rectTransform.position);
    }

    #endregion

    #region UI Callbacks

    private void onPlayerReadyButtonClicked () {
        buttonClicked = !buttonClicked;
        if (buttonClicked) {
            room.photonView.RPC("PlayerIsReady", RpcTarget.All, new object[] { playerName });
        } else {
           room.photonView.RPC("PlayerNotReady", RpcTarget.All, new object[] { playerName });
        }
    }

    #endregion

    #region Properties

    public Vector3 Position {
        get => rectTransform.position;
        set {
            rectTransform.localPosition = value;
        }
    }

    public bool Visible {
        set => gameObject.SetActive (value);
    }

    public string PlayerName {
        get => playerNameText.text;
        set {
            playerName = value;
            playerNameText.text = value;

            bool isLocalPlayer = PhotonNetwork.LocalPlayer.NickName.Equals(playerName);
            gameObject.SetActive(isLocalPlayer);
        }
    }

    public string Text {
        set => playerNameText.text = value;
    }

    public RoomPanel Room {
        get => room;
        set => room = value;
    }

    #endregion

}
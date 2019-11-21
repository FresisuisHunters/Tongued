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

    public override string ToString() {
        return string.Format("ROOM_PLAYER_ENTRY: {0}, {1}", playerName, rectTransform.position);
    }

    #endregion

    #region UI Callbacks

    private void onPlayerReadyButtonClicked () {
        buttonClicked = !buttonClicked;
        if (buttonClicked) {
            room.PlayerIsReady (playerName);
            playerNameText.text = string.Format ("* {0}", playerName);
        } else {
            room.PlayerNotReady (playerName);
            playerNameText.text = playerName;
        }
    }

    #endregion

    #region Properties

    public Vector3 Position {
        get => rectTransform.position;
        set {
            rectTransform.position.Set (value.x, value.y, value.z);
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

    public RoomPanel Room {
        get => room;
        set => room = value;
    }

    #endregion

}
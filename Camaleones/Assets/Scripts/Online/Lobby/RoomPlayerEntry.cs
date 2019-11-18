using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RoomPlayerEntry : MonoBehaviour {

    #region Private Fields

    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private Button playerReadyButton;
    private RoomPanel room;
    private string playerName;
    private bool buttonClicked;

    #endregion

    #region Unity Callbacks

    private void Awake() {
        buttonClicked = false;
        playerReadyButton.onClick.AddListener(() => onPlayerReadyButtonClicked());
    }

    #endregion

    #region UI Callbacks

    private void onPlayerReadyButtonClicked() {
        buttonClicked = !buttonClicked;
        if (buttonClicked) {
            room.PlayerIsReady(playerName);
            playerNameText.text = string.Format("* {0}", playerName);
        } else {
            room.PlayerNotReady(playerName);
            playerNameText.text = playerName;
        }
    }

    #endregion

    #region Properties

    public Vector3 Position {
        get => transform.position;
        set => transform.position.Set(value.x, value.y, value.z);
    }

    public bool Visible {
        set => gameObject.SetActive(value);
    }

    public string PlayerName {
        get => playerNameText.text;
        set {
            playerName = value;
            playerNameText.text = value;
        }
    }

    public RoomPanel Room {
        get => room;
        set => room = value;
    }

    #endregion

}
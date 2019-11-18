using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RoomPlayerEntry : MonoBehaviour {

    #region Private Fields

    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private Button playerReadyButton;
    private RoomPanel room;
    private bool buttonClicked;

    #endregion

    #region Unity Callbacks

    private void Awake() {
        playerReadyButton.onClick.AddListener(() => onPlayerReadyButtonClicked());
    }

    #endregion

    #region UI Callbacks

    private void onPlayerReadyButtonClicked() {
        string playerName = playerNameText.text;
        if (buttonClicked) {
            room.PlayerIsReady(playerName);
        } else {
            room.PlayerNotReady(playerName);
        }

        buttonClicked = !buttonClicked;
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
        set => playerNameText.text = value;
    }

    public RoomPanel Room {
        get => room;
        set => room = value;
    }

    #endregion

}
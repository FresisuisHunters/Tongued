using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;

public class JoinPrivateRoomPanel : MonoBehaviourPunCallbacks
{

    #region Private Fields

    [SerializeField] private TextMeshProUGUI errorMessageText;
    [SerializeField] private TMP_InputField roomNameInputField;
    [SerializeField] private Button attempToJoinRoomButton;
    [SerializeField] private Button quitJoinPrivateRoomPanelButton;

    #endregion

    #region Unity Callbacks

    private void Awake() {
        errorMessageText.text = "";
        attempToJoinRoomButton.onClick.AddListener(() => OnAttempToJoinRoomButtonClicked());
        quitJoinPrivateRoomPanelButton.onClick.AddListener(() => OnQuitJoinPrivateRoomButtonClicked());
    }

    protected new void OnEnable() {
        base.OnEnable();

        MakeUIInteractable();
    }

    #endregion

    #region Photon Callbacks

    public override void OnJoinedRoom() {
        OnlineLobbyManager.Instance.SwitchToRoomPanel();
    }

    public override void OnJoinRoomFailed(short returnCode, string message) {
        string log = string.Format("Couldn't join room. Return code: {0}. Error message: {1}", returnCode, message);
        OnlineLogging.Instance.Write(log);
    
        MakeUIInteractable();
        errorMessageText.text = message;
    }

    #endregion

    #region Private Methods

    private void MakeUIInteractable() {
        roomNameInputField.interactable = true;
        attempToJoinRoomButton.interactable = true;
        quitJoinPrivateRoomPanelButton.interactable = true;
    }

    private void MakeUINonInteractable() {
        roomNameInputField.interactable = false;
        attempToJoinRoomButton.interactable = false;
        quitJoinPrivateRoomPanelButton.interactable = false;
    }

    #endregion

    #region UI Callbacks

    private void OnAttempToJoinRoomButtonClicked() {
        string roomName = roomNameInputField.text;
        if (string.IsNullOrEmpty(roomName) || string.IsNullOrWhiteSpace(roomName)) {
            return;
        }

        MakeUINonInteractable();
        PhotonNetwork.JoinRoom(roomName);
    }

    private void OnQuitJoinPrivateRoomButtonClicked() {
        OnlineLobbyManager.Instance.SwitchToLobbyMainMenu();
    }

    #endregion

}

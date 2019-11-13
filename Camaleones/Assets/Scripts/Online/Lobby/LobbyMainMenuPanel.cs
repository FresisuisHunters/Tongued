using UnityEngine;
using UnityEngine.UI;

public class LobbyMainMenuPanel : MonoBehaviour {

    #region Private Fields

    [SerializeField] private Button joinPublicGameButton;
    [SerializeField] private Button createPrivateRoomButton;
    [SerializeField] private Button joinPrivateRoomButton;
    [SerializeField] private Button quitLobbyMenuButton;

    #endregion

    #region Unity Callbacks

    private void Awake () {
        joinPublicGameButton.onClick.AddListener (() => OnJoinPublicGameButtonClicked ());
        createPrivateRoomButton.onClick.AddListener (() => OnCreatePrivateRoomButtonClicked ());
        joinPrivateRoomButton.onClick.AddListener (() => OnJoinPrivateRoomButtonClicked ());
        quitLobbyMenuButton.onClick.AddListener (() => OnQuitLobbyMenuButtonClicked ());
    }

    #endregion

    #region UI Callbacks

    private void OnJoinPublicGameButtonClicked () {
        // TODO
    }

    private void OnCreatePrivateRoomButtonClicked () {
        OnlineLobbyManager.Instance.SwitchToCreateRoomPanel ();
    }

    private void OnJoinPrivateRoomButtonClicked () {
        OnlineLobbyManager.Instance.SwitchToJoinPrivateRoomPanel();
    }

    private void OnQuitLobbyMenuButtonClicked () {
        OnlineLobbyManager.Instance.SwitchToAskUsernamePanel ();
    }

    #endregion

}
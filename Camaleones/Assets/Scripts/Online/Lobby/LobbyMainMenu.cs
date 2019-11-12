using UnityEngine;
using UnityEngine.UI;

public class LobbyMainMenu : MonoBehaviour
{
    
    #region Public Fields

    public Button joinPublicGameButton;
    public Button createPrivateRoomButton;
    public Button joinPrivateRoomButton;
    public Button quitLobbyMenuButton;

    #endregion

    #region Unity Callbacks

    private void Awake() {
        joinPublicGameButton.onClick.AddListener(() => OnJoinPublicGameButtonClicked());
        createPrivateRoomButton.onClick.AddListener(() => OnCreatePrivateRoomButtonClicked());
        joinPrivateRoomButton.onClick.AddListener(() => OnJoinPrivateRoomButtonClicked());
        quitLobbyMenuButton.onClick.AddListener(() => OnQuitLobbyMenuButtonClicked());
    }

    #endregion

    #region UI Callbacks

    private void OnJoinPublicGameButtonClicked() {
        // TODO
    }

    private void OnCreatePrivateRoomButtonClicked() {
        OnlineLobbyManager.Instance.SwitchToCreateRoomPanel();
    }

    private void OnJoinPrivateRoomButtonClicked() {
    }

    private void OnQuitLobbyMenuButtonClicked() {

    }

    #endregion

}

using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using UnityEngine.SceneManagement;

#pragma warning disable 649
public class AskUsernamePanel : MonoBehaviourPunCallbacks {

    #region Private Fields

    [SerializeField] private TMP_InputField usernameInputField;
    [SerializeField] private Button connectToServerButton;
    [SerializeField] private Button goToMainMenuButton;
    [SerializeField] private SceneReference mainMenuScene;

    #endregion

    #region Unity Callbacks

    private void Awake () {
        connectToServerButton.onClick.AddListener (() => OnConnectToServerButtonClicked ());
        goToMainMenuButton.onClick.AddListener (() => OnGoToMainMenuButtonClicked ());
        usernameInputField.keyboardType = TouchScreenKeyboardType.ASCIICapable;
    }

    protected new void OnEnable() {
        base.OnEnable();
        
        MakeUIInteractable();
    }

    #endregion

    #region Photon Callbacks

    public override void OnConnectedToMaster () {
        OnlineLobbyManager.Instance.SwitchToLobbyMainMenu ();
    }

    #endregion

    #region Private Methods

    private void MakeUIInteractable() {
        usernameInputField.interactable = true;
        connectToServerButton.interactable = true;
        goToMainMenuButton.interactable = true;
    }

    private void MakeUINonInteractable() {
        usernameInputField.interactable = false;
        connectToServerButton.interactable = false;
        goToMainMenuButton.interactable = false;
    }

    private void ConnectToPhotonServer () {
        if (PhotonNetwork.IsConnected) {
            OnlineLogging.Instance.Write("Player already connected to Photon server");
            return;
        }

        PhotonNetwork.GameVersion = ServerConstants.GAME_VERSION;
        PhotonNetwork.ConnectUsingSettings ();
    }

    #endregion

    #region UI Callbcaks

    private void OnConnectToServerButtonClicked () {
        string username = usernameInputField.text;
        if (string.IsNullOrEmpty (username) || string.IsNullOrWhiteSpace (username)) {
            return;
        }

        MakeUINonInteractable();
        PhotonNetwork.LocalPlayer.NickName = username;
        ConnectToPhotonServer ();
    }

    private void OnGoToMainMenuButtonClicked () {

        SceneManagerExtensions.LoadScene(mainMenuScene, LoadSceneMode.Single, () =>
            FindObjectOfType<MenuScreenManager>().startingMenuScreen = FindObjectOfType<MainMenuScreen>());
    }

    #endregion

}
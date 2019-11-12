using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AskUsernamePanel : MonoBehaviourPunCallbacks {

    #region Private Fields

    [SerializeField] private TMP_InputField usernameInputField;
    [SerializeField] private Button connectToServerButton;
    [SerializeField] private Button goToMainMenuButton;

    #endregion

    #region Unity Callbacks

    private void Awake () {
        connectToServerButton.onClick.AddListener (() => OnConnectToServerButtonClicked ());
        goToMainMenuButton.onClick.AddListener (() => OnGoToMainMenuButtonClicked ());
    }

    #endregion

    #region Photon Callbacks

    public override void OnConnectedToMaster () {
        OnlineLobbyManager.Instance.SwitchToLobbyMainMenu ();
    }

    #endregion

    #region Private Methods

    private void ConnectToPhotonServer () {
        if (PhotonNetwork.IsConnected) {
            Debug.LogError ("Player already connected to Photon server");
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

        PhotonNetwork.LocalPlayer.NickName = username;
        ConnectToPhotonServer ();
    }

    private void OnGoToMainMenuButtonClicked () {
        Debug.LogWarning ("TODO: Implementar menú principal");
    }

    #endregion

}
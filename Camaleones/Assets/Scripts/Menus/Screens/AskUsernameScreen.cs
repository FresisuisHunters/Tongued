using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

#pragma warning disable 649
public class AskUsernameScreen : AMenuScreen
{
    [SerializeField] private SceneReference mainMenuScene;
    [SerializeField] private TMPro.TMP_InputField usernameInputField;
    [SerializeField] private Button backButton;

    public override void GoBack()
    {
        PhotonNetwork.Disconnect();
        SceneManagerExtensions.LoadScene(mainMenuScene, LoadSceneMode.Single, () =>
            FindObjectOfType<MenuScreenManager>().startingMenuScreen = FindObjectOfType<MainMenuScreen>());
    }

    public void GoToLobbyScreen()
    {
        MenuManager.SetActiveMenuScreen<LobbyScreen>();
    }

    public void ConnectToPhotonServer()
    {
        string username = usernameInputField.text;
        if (string.IsNullOrEmpty(username) || string.IsNullOrWhiteSpace(username))
        {
            return;
        }
        PhotonNetwork.LocalPlayer.NickName = username;

        if (PhotonNetwork.IsConnected)
        {
            OnlineLogging.Instance.Write("Player already connected to Photon server");
            return;
        }

        PhotonNetwork.GameVersion = ServerConstants.GAME_VERSION;
        PhotonNetwork.ConnectUsingSettings();

        SetInteractable(false, backButton);
    }
}

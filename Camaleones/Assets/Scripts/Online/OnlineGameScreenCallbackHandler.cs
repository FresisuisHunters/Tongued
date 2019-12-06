using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class OnlineGameScreenCallbackHandler : MonoBehaviourPunCallbacks
{
    [SerializeField] private SceneReference lobbyScene;
    [SerializeField] private SceneReference mainMenuScene;

    public override void OnLeftRoom() => GoToLobby();

    public override void OnDisconnected(DisconnectCause cause) => GoToMainMenu();


    private void GoToLobby()
    {
        SceneManagerExtensions.LoadScene(lobbyScene, UnityEngine.SceneManagement.LoadSceneMode.Single, () =>
            FindObjectOfType<MenuScreenManager>().startingMenuScreen = FindObjectOfType<LobbyScreen>());
    }

    private void GoToMainMenu()
    {
        SceneManagerExtensions.LoadScene(mainMenuScene, UnityEngine.SceneManagement.LoadSceneMode.Single, () =>
               FindObjectOfType<MenuScreenManager>().startingMenuScreen = FindObjectOfType<MainMenuScreen>());
    }
}

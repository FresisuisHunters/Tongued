using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#pragma warning disable 649
[RequireComponent (typeof (Button))]
public class ExitGameplaySceneButton : MonoBehaviourPunCallbacks {
    private enum SceneToLoadOnDisconnect {
        ONLINE_MAIN_MENU,
        LOBBY_MENU
    }

    [SerializeField] private GameObject confirmScreen;
    [SerializeField] private SceneReference mainMenuScene;
    [SerializeField] private SceneReference lobbyScene;
    private SceneToLoadOnDisconnect sceneToLoad;

    private void OnApplicationQuit () {
        OnlineLogging.Instance.Close ();
    }

    public void ToggleConfirmScreen () {
        confirmScreen.SetActive (!confirmScreen.activeSelf);
    }

    public override void OnMasterClientSwitched (Player newMasterClient) {
        OnlineLogging.Instance.Write ("New master player: " + newMasterClient.ActorNumber);
    }

    public override void OnPlayerLeftRoom (Player otherPlayer) {
        OnlineLogging.Instance.Write (otherPlayer.ActorNumber + " left room and game");
    }

    public override void OnLeftRoom () {
        OnlineLogging.Instance.Write ("Left room and game");
        OnlineLogging.Instance.Write ("Id of player who left: " + PhotonNetwork.LocalPlayer.ActorNumber);

        switch (sceneToLoad) {
            case SceneToLoadOnDisconnect.LOBBY_MENU:
                SceneManager.LoadScene (lobbyScene, LoadSceneMode.Single);
                break;
            case SceneToLoadOnDisconnect.ONLINE_MAIN_MENU:
                SceneManagerExtensions.LoadScene (mainMenuScene, UnityEngine.SceneManagement.LoadSceneMode.Single, () =>
                    FindObjectOfType<MenuScreenManager> ().startingMenuScreen = FindObjectOfType<MainMenuScreen> ());
                break;
        }
    }

    public void GoBackToMainMenu () {
        if (PhotonNetwork.IsConnected) {
            sceneToLoad = SceneToLoadOnDisconnect.ONLINE_MAIN_MENU;
            OnlineLogging.Instance.Write ("Id of player who left: " + PhotonNetwork.LocalPlayer.ActorNumber);
            PhotonNetwork.LeaveRoom ();
        } else {
            SceneManagerExtensions.LoadScene (mainMenuScene, UnityEngine.SceneManagement.LoadSceneMode.Single, () =>
                FindObjectOfType<MenuScreenManager> ().startingMenuScreen = FindObjectOfType<MainMenuScreen> ());
        }
    }

    public void GoBackToTrainingScreen () {
        SceneManagerExtensions.LoadScene (mainMenuScene, UnityEngine.SceneManagement.LoadSceneMode.Single, () =>
            FindObjectOfType<MenuScreenManager> ().startingMenuScreen = FindObjectOfType<TrainingMenuScreen> ());
    }

    public void GoToLobby () {
        sceneToLoad = SceneToLoadOnDisconnect.LOBBY_MENU;
        OnlineLogging.Instance.Write ("Id of player who left: " + PhotonNetwork.LocalPlayer.ActorNumber);
        PhotonNetwork.LeaveRoom ();
    }

}
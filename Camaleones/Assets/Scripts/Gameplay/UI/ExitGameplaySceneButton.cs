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

    public override void OnDisconnected(DisconnectCause cause) {
        GoToMainMenu();
    }

    public override void OnLeftRoom () {
        // TODO: Ir al lobby
        PhotonNetwork.Disconnect();
    }

    public void GoBackToTrainingScreen () {
        SceneManagerExtensions.LoadScene (mainMenuScene, UnityEngine.SceneManagement.LoadSceneMode.Single, () =>
            FindObjectOfType<MenuScreenManager> ().startingMenuScreen = FindObjectOfType<TrainingMenuScreen> ());
    }

    public void QuitGame() {
        if (PhotonNetwork.IsConnectedAndReady) {
            PhotonNetwork.LeaveRoom();
        } else {
            GoToMainMenu();
        }
    }

    private void GoToMainMenu() {
        SceneManagerExtensions.LoadScene (mainMenuScene, UnityEngine.SceneManagement.LoadSceneMode.Single, () =>
                FindObjectOfType<MenuScreenManager> ().startingMenuScreen = FindObjectOfType<MainMenuScreen> ());
    }

}
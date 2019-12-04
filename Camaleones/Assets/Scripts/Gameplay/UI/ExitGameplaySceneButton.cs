using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;

#pragma warning disable 649
[RequireComponent(typeof(Button))]
public class ExitGameplaySceneButton : MonoBehaviourPunCallbacks
{
    private enum SceneToLoadOnDisconnect {
        ONLINE_MAIN_MENU,
        LOBBY_MENU
    }

    [SerializeField] private GameObject confirmScreen;
    [SerializeField] private SceneReference mainMenuScene;
    [SerializeField] private SceneReference lobbyScene;
    private SceneToLoadOnDisconnect sceneToLoad;
    
    private void OnApplicationQuit() {
        OnlineLogging.Instance.Close();
    }

    public void ToggleConfirmScreen()
    {
        confirmScreen.SetActive(!confirmScreen.activeSelf);
    }

    public override void OnLeftRoom() {
        OnlineLogging.Instance.Write("Left room and game");

        switch (sceneToLoad) {
            case SceneToLoadOnDisconnect.LOBBY_MENU:
                SceneManager.LoadScene(lobbyScene, LoadSceneMode.Single);
                break;
            case SceneToLoadOnDisconnect.ONLINE_MAIN_MENU:
                SceneManagerExtensions.LoadScene(mainMenuScene, UnityEngine.SceneManagement.LoadSceneMode.Single, () =>
                    FindObjectOfType<MenuScreenManager>().startingMenuScreen = FindObjectOfType<MainMenuScreen>());
                break;
        }
    }

    public void GoBackToMainMenu()
    {
        if (PhotonNetwork.IsConnected) {
            sceneToLoad = SceneToLoadOnDisconnect.ONLINE_MAIN_MENU;
            PhotonNetwork.LeaveRoom();
        } else {
            SceneManagerExtensions.LoadScene(mainMenuScene, UnityEngine.SceneManagement.LoadSceneMode.Single, () =>
            FindObjectOfType<MenuScreenManager>().startingMenuScreen = FindObjectOfType<MainMenuScreen>());
        }
    }

    public void GoBackToTrainingScreen()
    {
        SceneManagerExtensions.LoadScene(mainMenuScene, UnityEngine.SceneManagement.LoadSceneMode.Single, () =>
            FindObjectOfType<MenuScreenManager>().startingMenuScreen = FindObjectOfType<TrainingMenuScreen>());
    }

    public void GoToLobby()
    {
        sceneToLoad = SceneToLoadOnDisconnect.LOBBY_MENU;
        PhotonNetwork.LeaveRoom();
    }

    
}

using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#pragma warning disable 649
[RequireComponent (typeof (Button))]
public class ExitGameplaySceneButton : MonoBehaviour {

    [SerializeField] private GameObject confirmScreen;
    [SerializeField] private SceneReference mainMenuScene;


    public void ToggleConfirmScreen()
    {
        confirmScreen.SetActive (!confirmScreen.activeSelf);
    }

    public void QuitGame()
    {
        if (PhotonNetwork.IsConnectedAndReady) PhotonNetwork.LeaveRoom();
        else GoToMainMenu();
    }


    private void GoToMainMenu()
    {
        SceneManagerExtensions.LoadScene(mainMenuScene, UnityEngine.SceneManagement.LoadSceneMode.Single, () =>
               FindObjectOfType<MenuScreenManager>().startingMenuScreen = FindObjectOfType<MainMenuScreen>());
    }
}
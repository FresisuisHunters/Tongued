using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ExitGameplaySceneButton : MonoBehaviour
{
    [SerializeField] private GameObject confirmScreen;
    [SerializeField] private SceneReference mainMenuScene;
    [SerializeField] private SceneReference lobbyScene;

    public void ToggleConfirmScreen()
    {
        confirmScreen.SetActive(!confirmScreen.activeSelf);
    }

    public void GoBackToMainMenu()
    {
        SceneManagerExtensions.LoadScene(mainMenuScene, UnityEngine.SceneManagement.LoadSceneMode.Single, () =>
            FindObjectOfType<MenuScreenManager>().startingMenuScreen = FindObjectOfType<MainMenuScreen>());
    }

    public void GoBackToTrainingScreen()
    {
        SceneManagerExtensions.LoadScene(mainMenuScene, UnityEngine.SceneManagement.LoadSceneMode.Single, () =>
            FindObjectOfType<MenuScreenManager>().startingMenuScreen = FindObjectOfType<TrainingMenuScreen>());
    }

    public void GoToLobby()
    {
        SceneManager.LoadScene(lobbyScene, LoadSceneMode.Single);
    }

    
}

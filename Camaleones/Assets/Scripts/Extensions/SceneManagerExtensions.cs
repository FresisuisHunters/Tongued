using System;
using UnityEngine.SceneManagement;
using Photon.Pun;

public static class SceneManagerExtensions
{
    /// <summary>
    /// Permite cargar una escena y ejecutar una función justo después sin un montón de boilerplate para el callback.
    /// </summary>
    public static void LoadScene(SceneReference scene, LoadSceneMode loadSceneMode, Action onSceneActiveCallback)
    {
        SceneManager.LoadScene(scene, LoadSceneMode.Single);

        SceneManager.activeSceneChanged += CallbackInvoker;

        void CallbackInvoker(Scene previousScene, Scene newScene)
        {
            SceneManager.activeSceneChanged -= CallbackInvoker;

            onSceneActiveCallback();
        }
    }

    public static void LoadScene(string sceneName, LoadSceneMode loadSceneMode, Action onSceneActiveCallback)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);

        SceneManager.activeSceneChanged += CallbackInvoker;

        void CallbackInvoker(Scene previousScene, Scene newScene)
        {
            SceneManager.activeSceneChanged -= CallbackInvoker;

            onSceneActiveCallback();
        }
    }

    public static void PhotonLoadScene(string sceneName, Action onSceneActiveCallback)
    {
        
        PhotonNetwork.LoadLevel(sceneName);

        SceneManager.activeSceneChanged += CallbackInvoker;

        void CallbackInvoker(Scene previousScene, Scene newScene)
        {
            SceneManager.activeSceneChanged -= CallbackInvoker;

            onSceneActiveCallback();
        }
    }
}

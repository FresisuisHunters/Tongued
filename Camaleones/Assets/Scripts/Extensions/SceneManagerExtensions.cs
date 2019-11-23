using System;
using UnityEngine.SceneManagement;

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
}

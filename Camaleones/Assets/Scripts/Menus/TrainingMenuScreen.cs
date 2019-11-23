using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#pragma warning disable 649
[RequireComponent(typeof(CanvasGroup))]
public class TrainingMenuScreen : AMenuScreen
{
    [SerializeField] SceneReference tmpTrainingScene;

    protected override void OnOpen(System.Type previousScreen)
    {
        SceneManager.LoadScene(tmpTrainingScene);
    }
}

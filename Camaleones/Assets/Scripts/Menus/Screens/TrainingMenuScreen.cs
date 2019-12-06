using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#pragma warning disable 649
[RequireComponent(typeof(CanvasGroup))]
public class TrainingMenuScreen : AMenuScreen
{
    [SerializeField] SceneReference trainingScene;
    [SerializeField] GameObject playerPrefab;

    protected override void OnOpen(System.Type previousScreen)
    {
        SceneManagerExtensions.LoadScene(trainingScene, LoadSceneMode.Single, () =>
        {
            Transform spawnPoint = GameObject.FindGameObjectWithTag("PlayerSpawnPoint").transform;

            GameObject player = Instantiate(playerPrefab, new Vector3(spawnPoint.position.x, spawnPoint.position.y, 0), Quaternion.identity);
            Destroy(player.GetComponentInChildren<PlayerNameAndScoreDisplay>().gameObject);
        });
    }
}

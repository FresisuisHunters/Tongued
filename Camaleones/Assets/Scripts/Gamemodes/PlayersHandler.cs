using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 649

/// <summary>
/// 
/// </summary>
public class PlayersHandler : MonoBehaviour
{
    #region Inspector
    [Tooltip("")]
    [Range(1,10)]
    public int playerNumber;
    [Tooltip("")]
    [SerializeField] private GameObject mainPlayerPrefab;
    [Tooltip("")]
    [SerializeField] private GameObject playerPrefab;
    #endregion

    #region Private Variables
    private List<GameObject> playerList;
    private List<GameObject> spawnPoints;
    #endregion

    // Start is called before the first frame update
    void Awake()
    {
        playerList = new List<GameObject>(){Instantiate(mainPlayerPrefab)};
        for (int i = 0; i < playerNumber - 1; i++)
            playerList.Add(Instantiate(playerPrefab));
        spawnPoints = new List<GameObject>();
        spawnPoints.AddRange(GameObject.FindGameObjectsWithTag("PlayerSpawnPoint"));

        SpawnPlayers();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 
    /// </summary>
    public void SpawnPlayers()
    {
        List<GameObject> temp = new List<GameObject>();
        foreach(GameObject g in playerList)
            temp.Add(g);
        foreach(GameObject t in spawnPoints)
        {
            int randPos = Random.Range(0, temp.Count);
            GameObject tempPlayer = temp[randPos];
            tempPlayer.transform.position = t.transform.position;
            temp.RemoveAt(randPos);
        }
        foreach (Hook h in FindObjectsOfType<Hook>())
            h.Disable();
    }
}

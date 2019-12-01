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
    [Tooltip("Numero de jugadores en la partida")]
    [Range(1,10)]
    public int playerNumber;
    [Tooltip("Prefab que usa el jugador principal")]
    [SerializeField] protected GameObject mainPlayerPrefab;
    [Tooltip("Prefabs usados para pruebas u otros jugadores en offline")]
    [SerializeField] private GameObject playerPrefab;
    #endregion

    #region Private Variables
    public IEnumerable Players => playerList;
    private List<GameObject> playerList;
    #endregion
    
    #region Protected Variables
    //Lista de puntos donde pueden spawnear los jugadores
    protected List<GameObject> spawnPoints;
    #endregion

    protected virtual void Awake()
    {
        spawnPoints = new List<GameObject>();
        spawnPoints.AddRange(GameObject.FindGameObjectsWithTag("PlayerSpawnPoint"));

        SpawnPlayers();
    }

    /// <summary>
    /// Método que spawnea a todos los jugadores
    /// </summary>
    public virtual void SpawnPlayers()
    {
        playerList = new List<GameObject>() { Instantiate(mainPlayerPrefab) };
        for (int i = 0; i < playerNumber - 1; i++)
            playerList.Add(Instantiate(playerPrefab));
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
    }
}

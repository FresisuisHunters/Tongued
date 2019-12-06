using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

/// <summary>
/// Clase que gestiona tareas relacionadas con los jugadores de la partida
/// </summary>
[RequireComponent(typeof(PhotonView))]
public class OnlinePlayersHandler : PlayersHandler
{
    private PhotonView photonView;

    protected override void Awake()
    {
        photonView = GetComponent<PhotonView>();
        base.Awake();
    }


    public void RegisterSpawnedPlayer(OnlinePlayer registeredPlayer)
    {
        spawnedPlayerCount++;
        playerList.Add(registeredPlayer.gameObject);
    }

    /// <summary>
    /// Este método instancia a los jugadores desde master
    /// </summary>
    public override void SpawnPlayers()
    {
        playerList = new List<GameObject>();
        playerNumber = PhotonNetwork.CurrentRoom.PlayerCount;

        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            List<GameObject> spawnPointPool = new List<GameObject>();
            foreach (GameObject g in spawnPoints)
                spawnPointPool.Add(g);

            foreach (Player p in PhotonNetwork.PlayerList)
            {
                Vector2 position;
                if (spawnPointPool.Count > 0)
                {
                    int randIndex = Random.Range(0, spawnPointPool.Count);
                    position = spawnPointPool[randIndex].transform.position;
                    spawnPointPool.RemoveAt(randIndex);
                }
                else
                {
                    position = spawnPoints[Random.Range(0, spawnPoints.Count)].transform.position;
                    Debug.LogWarning("No hay suficientes spawnpoints en este mapa.", this);
                }

                photonView.RPC("SpawnPlayerOnline", p, position);
            }
        }
    }

    /// <summary>
    /// Este método instancia a un jugador en la posición recibida de master
    /// </summary>
    [PunRPC]
    private void SpawnPlayerOnline(Vector2 position)
    {
        string prefabName = mainPlayerPrefab.name;
        Quaternion rotation = mainPlayerPrefab.transform.rotation;

        GameObject player = PhotonNetwork.Instantiate("OnlinePlayer", position, rotation, 0);
        player.name = PhotonNetwork.LocalPlayer.NickName;

        playerList.Add(player);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
public class OnlinePlayersHandler : PlayersHandler
{
    protected override void Awake()
    {

        spawnPoints = new List<GameObject>();
        spawnPoints.AddRange(GameObject.FindGameObjectsWithTag("PlayerSpawnPoint"));
    }

    public override void SpawnPlayers()
    {
        base.SpawnPlayers();
    }
}

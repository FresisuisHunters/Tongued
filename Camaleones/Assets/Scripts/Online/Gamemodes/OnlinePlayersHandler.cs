using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// 
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

    /// <summary>
    /// 
    /// </summary>
    public override void SpawnPlayers()
    {
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            List<GameObject> temp = new List<GameObject>();
            foreach (Gameobject g in spawnPoints)
                temp.Add(g);
            foreach (Player p in PhotonNetwork.otherPlayers)
            {
                int randPos = Random.Range(0, temp.Count);
                Vector2 position = temp[randPos].transform.position;
                photonView.RPC("SpawnPlayerOnline", p, position);
                temp.RemoveAt(randPos);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="position"></param>
    [PunRPC]
    private void SpawnPlayerOnline(Vector2 position)
    {
        string prefabName = mainPlayerPrefab.name;
        Quaternion rotation = mainPlayerPrefab.transform.rotation;

        GameObject player = PhotonNetwork.Instantiate(mainPlayerPrefab, position, rotation, 0);
        player.name = PhotonNetwork.LocalPlayer.NickName;
    }
}

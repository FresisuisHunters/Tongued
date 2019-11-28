using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
public class OnlineScoreHandler : ScoreHandler
{

    private PhotonView photonView;

    protected override void Awake()
    {
        photonView = GetComponent<PhotonView>();
        base.Awake();
    }

    /// <summary>
    /// Metodo que suma la puntuacion al jugador con la snitch
    /// </summary>
    /// <param name="score"></param>
    public override void AddScore(int score)
    {
        photonView.RPC("RPCAddScore", RpcTarget.Others, score);
        base.AddScore(score);
    }

    [PunRPC]
    private void RPCAddScore(int score)
    {
        base.AddScore(score);
    }
}

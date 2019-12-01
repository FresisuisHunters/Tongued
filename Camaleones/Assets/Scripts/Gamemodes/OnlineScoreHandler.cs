using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
public class OnlineScoreHandler : ScoreHandler
{
    private PhotonView photonView;

   
    public override void AddScore(int diff)
    {
        base.AddScore(diff);

        photonView.RPC("RPC_SetScore", RpcTarget.Others, CurrentScore);
    }
    [PunRPC]
    private void RPC_SetScore(int score)
    {
        CurrentScore = score;
    }


    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }
}

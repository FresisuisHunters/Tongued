using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

[RequireComponent(typeof(PhotonView))]
public class OnlineHotPotatoHandler : HotPotatoHandler, IPunObservable
{
    private PhotonView photonView;


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(TimeLeftInRound);
        }
        else
        {
            TimeLeftInRound = (float) stream.ReceiveNext();
        }
    }


    protected override void StartRound(RoundType roundType)
    {
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            base.StartRound(roundType);
        }

        photonView.RPC("RPC_StartHotPotatoRound", RpcTarget.Others, roundType);
    }
    [PunRPC]
    private void RPC_StartHotPotatoRound(RoundType roundType)
    {
        base.StartRound(roundType);
    }


    protected override void EndRound()
    {
        //Los clientes no master nunca ejecutan EndRound.
        if (!PhotonNetwork.LocalPlayer.IsMasterClient) return;

        base.EndRound();
    }

    protected override void EndMatch()
    {
        if (!PhotonNetwork.LocalPlayer.IsMasterClient) return;
        base.EndMatch();
        photonView.RPC("RPC_EndHotPotatoMatch", RpcTarget.Others);
    }
    [PunRPC]
    private void RPC_EndHotPotatoMatch()
    {
        base.EndMatch();
    }



    #region Initialization
    protected override void Awake()
    {
        photonView = GetComponent<PhotonView>();

        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
            SpawnSnitch();
        }

        playersHandler = GetComponent<PlayersHandler>();
    }

    protected override void SpawnSnitch()
    {
        GameObject spawnPoint = GameObject.FindGameObjectWithTag("SnitchSpawnPoint");
        if (!spawnPoint)
        {
            Debug.LogError("There is no SnitchSpawnPoint in the scene.");
        }

        snitch = PhotonNetwork.Instantiate("OnlineSnitch", spawnPoint.transform.position, Quaternion.identity, 0).GetComponent<TransferableItem>();
        snitch.OnItemTransfered += OnSnitchTransfered;
    }
    #endregion
}

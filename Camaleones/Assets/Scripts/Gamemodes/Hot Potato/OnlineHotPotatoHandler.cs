using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PhotonView))]
public class OnlineHotPotatoHandler : HotPotatoHandler, IPunObservable
{
    private PhotonView photonView;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(TimeLeftInRound);
            stream.SendNext(RoundDurationSinceLastReset);
        }
        else
        {
            TimeLeftInRound = (float) stream.ReceiveNext();
            RoundDurationSinceLastReset = (float) stream.ReceiveNext();
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
        photonView.RPC("RPC_EndHotPotatoMatch", RpcTarget.Others);
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
            base.EndMatch();
    }

    protected override void GoToScoresScene(List<PlayerScoreData> scores) 
    {
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
            PhotonNetwork.LoadLevel(scoreSceneName);
    }
    [PunRPC]
    private void RPC_EndHotPotatoMatch()
    {
        Debug.Log("Se acabó wey");
        ScoreCollector scollector = Instantiate(scoreCollector).GetComponent<ScoreCollector>();
        scollector.CollectScores();
    }

    #region Initialization
    protected override void Awake()
    {
        base.Awake();

        photonView = GetComponent<PhotonView>();

        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            photonView.TransferOwnership(PhotonNetwork.LocalPlayer);

            PhotonView snitchPhotonView = Snitch.GetComponent<PhotonView>();
            snitchPhotonView.TransferOwnership(PhotonNetwork.LocalPlayer);
            PhotonNetwork.AllocateViewID(snitchPhotonView);

            photonView.RPC("RPC_SetSnitchViewID", RpcTarget.Others, snitchPhotonView.ViewID);
        }
    }

    [PunRPC]
    private void RPC_SetSnitchViewID(int id)
    {
        Snitch.GetComponent<PhotonView>().ViewID = id;
    }
    #endregion
}

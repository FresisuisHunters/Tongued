using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using ExitGames.Client.Photon;

[RequireComponent(typeof (PhotonView))]
public class OnlineHotPotatoHandler : HotPotatoHandler, IPunObservable, IInRoomCallbacks
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

    #region Rounds
    protected override void StartRound(RoundType roundType)
    {
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            base.StartRound(roundType);
        }

        photonView.RPC("RPC_StartHotPotatoRound", RpcTarget.Others, roundType, currentChanceOfSameRound);
    }
    [PunRPC]
    private void RPC_StartHotPotatoRound(RoundType roundType, float currentChanceOfSameRound)
    {
        this.currentChanceOfSameRound = currentChanceOfSameRound;
        base.StartRound(roundType);
    }

    protected override void EndRound()
    {
        //Los clientes no master nunca ejecutan EndRound.
        if (!PhotonNetwork.LocalPlayer.IsMasterClient) return;

        base.EndRound();
    }
    #endregion

    #region Match End
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
        ScoreCollector scollector = Instantiate(scoreCollector).GetComponent<ScoreCollector>();
        scollector.CollectScores();
    }

    protected override void GoToScoresScene(List<PlayerScoreData> scores)
    {
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
            SceneManagerExtensions.PhotonLoadScene(scoreSceneName, () => FindObjectOfType<ScoresScreen>().ShowScores(scores));
    }
    #endregion

    #region In Room Calbacks
    
    void IInRoomCallbacks.OnPlayerLeftRoom(Player otherPlayer)
    {
        if (PhotonNetwork.CountOfPlayersInRooms == 1)
        {
            Debug.Log("Am master:" + PhotonNetwork.IsMasterClient);
            EndMatch();
        }
    }

    void IInRoomCallbacks.OnPlayerEnteredRoom(Player newPlayer) { }
    void IInRoomCallbacks.OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged) { }
    void IInRoomCallbacks.OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps) { }
    void IInRoomCallbacks.OnMasterClientSwitched(Player newMasterClient) { }
    #endregion

    #region Initialization
    protected override void Awake ()
    {
        photonView = GetComponent<PhotonView> ();
        SpawnSnitch();

        if (PhotonNetwork.LocalPlayer.IsMasterClient) {
            PhotonView snitchPhotonView = Snitch.GetComponent<PhotonView>();
            PhotonNetwork.AllocateSceneViewID(snitchPhotonView);
            photonView.RPC ("RPC_SetSnitchViewID", RpcTarget.Others, snitchPhotonView.ViewID);
        }

        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDestroy()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    [PunRPC]
    private void RPC_SetSnitchViewID (int id) {
        Snitch.GetComponent<PhotonView>().ViewID = id;
    }
    #endregion
}
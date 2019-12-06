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
    public float countdownBeforeSpawn = 5f;

    [SerializeField] private GameObject inGameUI;
    [SerializeField] private TextMeshProUGUI countdownText;
    private PhotonView photonView;
    private bool gameHasStarted = false;
    private float currentCountdown;

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

    protected new void Update() {
        if (gameHasStarted) {
            base.Update();
        } else {
            currentCountdown -= Time.deltaTime;

            float clampedCountdown = Mathf.Max(currentCountdown, 0);
            countdownText.text = string.Format("Game starts in {0}...", Mathf.CeilToInt(clampedCountdown));

            if (currentCountdown <= 0f) {
                gameHasStarted = true;

                if (PhotonNetwork.LocalPlayer.IsMasterClient) {
                    photonView.RPC("SpawnOnlineSnitch", RpcTarget.All);
                }
            }
        }
    }

    [PunRPC]
    private void SpawnOnlineSnitch() {
        Debug.Log("SpawnOnlineSnitch");

        gameHasStarted = true;
        SpawnSnitch();

        if (PhotonNetwork.LocalPlayer.IsMasterClient) {
            PhotonView snitchPhotonView = Snitch.GetComponent<PhotonView>();
            PhotonNetwork.AllocateSceneViewID(snitchPhotonView);
            photonView.RPC ("RPC_SetSnitchViewID", RpcTarget.Others, snitchPhotonView.ViewID);
        }

        inGameUI.SetActive(true);
        GetComponent<HotPotatoUI>().OnSpawnCountdownEnded();
        countdownText.gameObject.SetActive(false);
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
        {
            PhotonNetwork.DestroyAll();
            PhotonNetwork.LoadLevel(scoreSceneName);
        }
    }
    [PunRPC]
    private void RPC_EndHotPotatoMatch()
    {
        ScoreCollector scollector = Instantiate(scoreCollector).GetComponent<ScoreCollector>();
        scollector.CollectScores();
    }
    #endregion

    #region In Room Calbacks    
    void IInRoomCallbacks.OnPlayerLeftRoom(Player otherPlayer)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
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
        StartSpawnCountdown();
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void StartSpawnCountdown() {
        currentCountdown = countdownBeforeSpawn;
        gameHasStarted = false;
    }

    private void OnDestroy()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void Start() {
        photonView = GetComponent<PhotonView> ();
    }

    [PunRPC]
    private void RPC_SetSnitchViewID (int id) {
        Debug.Log("SpawnOnlineSnitch");
        Snitch.GetComponent<PhotonView>().ViewID = id;
    }
    #endregion
}
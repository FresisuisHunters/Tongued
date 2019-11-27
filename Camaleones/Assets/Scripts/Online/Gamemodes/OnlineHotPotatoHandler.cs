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
            stream.SendNext(timerCurrentTime);
        }
        else
        {
            timerCurrentTime = (int)stream.ReceiveNext();
        }
    }
    protected override void Awake()
    {
        photonView = GetComponent<PhotonView>();
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            photonView.RequestOwnership();

            GameObject spawnPoint = GameObject.FindGameObjectWithTag("SnitchSpawnPoint");
            if (!spawnPoint)
            {
                Debug.LogError("There is no SnitchSpawnPoint in the scene.");
            }

            snitchReference = PhotonNetwork.Instantiate("OnlineSnitch", spawnPoint.transform.position, Quaternion.identity, 0).GetComponent<TransferableItem>();
        }

        timerText = GetComponentInChildren<TextMeshProUGUI>();

        roundType = true;
        currentRound = 0;
        roundChangeChance = 0.5f;

        playersHandler = GetComponent<PlayersHandler>();
    }
    protected override void UpdateTimer()
    {
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
            base.UpdateTimer();
        else
            timerText.SetText(timerCurrentTime.ToString());
    }

    public override void NotifyTransfer()
    {
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
            base.NotifyTransfer();
        else if (!hasStarted)
            hasStarted = true;
    }

    protected override void RestartTimer()
    {
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
            base.RestartTimer();
    }

    protected override void StartNewRound()
    {
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            base.StartNewRound();
            photonView.RPC("ChangeRoundType", RpcTarget.Others, roundType);
        }
    }

    [PunRPC]
    private void ChangeRoundType(bool type)
    {
        roundType = type;
    }

    protected override void Update()
    {
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
            base.Update();
        else if (hasStarted)
            UpdateTimer();
    }

}

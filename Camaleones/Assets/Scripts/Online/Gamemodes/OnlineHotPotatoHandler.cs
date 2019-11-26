using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
public class OnlineHotPotatoHandler : HotPotatoHandler, IPunObservable
{

    private PhotonView photonView;
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(Length);
        }
        else
        {
            Length = (float)stream.ReceiveNext();
        }
    }
    protected override void Awake()
    {
        photonView = GetComponent<PhotonView>();
        if(PhotonNetwork.LocalPlayer.IsMasterClient)
            photonView.RequestOwnership();
        base.Awake();
    }
}

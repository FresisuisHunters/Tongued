using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
public class OnlineTransferableItem : TransferableItem, IPunObservable, IPunInstantiateMagicCallback
{
    private PhotonView photonView;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        
    }


}

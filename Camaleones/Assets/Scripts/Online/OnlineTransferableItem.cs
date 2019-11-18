using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
public class OnlineTransferableItem : TransferableItem 
{
    private PhotonView photonView;

    protected override void TITransfer(GameObject target)
    {
        photonView.RPC("RPCTransfer", RpcTarget.Others, target.GetPhotonView().ViewID);
        base.TITransfer(target);
    }

    [PunRPC]
    private void RPCTransfer(int id)
    {
        GameObject target = PhotonView.Find(id).gameObject;
        base.TITransfer(target);
    }
}

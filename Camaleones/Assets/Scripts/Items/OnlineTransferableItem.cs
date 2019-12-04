using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
public class OnlineTransferableItem : TransferableItem 
{
    private PhotonView photonView;
    
    public override void Collide(GameObject collisionObject)
    {
        //Ignoramos esta colisión si este objeto no es nuestro
        if (collisionObject.GetComponent<PhotonView>()?.IsMine ?? false)
        {
            base.Collide(collisionObject);
        }
    }

    /// <summary>
    /// Mandamos al resto de jugadores el evento indicando que hemos cogido la snitch con la id de nuestro photonview y luego ejecutamos el código de la clase padre
    /// </summary>
    protected override void TITransfer(TransferableItemHolder target)
    {
        int targetId = target.GetComponent<PhotonView>().ViewID;
        photonView.RPC("RPCTransferOwnership", RpcTarget.MasterClient, targetId);
        photonView.RPC("RPCTransfer", RpcTarget.All, targetId);
    }

    [PunRPC]
    private void RPCTransfer(int id)
    {
        PhotonView targetPhotonView = PhotonView.Find(id);
        TransferableItemHolder target = targetPhotonView.GetComponent<TransferableItemHolder>();
        
        base.TITransfer(target);
    }

    [PunRPC]
    private void RPCTransferOwnership(int targetId) {
        OnlineLogging.Instance.Write("PASANDO TESTIGO A " + targetId);

        PhotonView targetPhotonView = PhotonView.Find(targetId);
        photonView.TransferOwnership(targetPhotonView.Owner);
    }


    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }
}

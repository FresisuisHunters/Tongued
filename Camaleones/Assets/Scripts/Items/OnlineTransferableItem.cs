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
        int targetId = target?.GetComponent<PhotonView>().ViewID ?? -1;
        photonView.RPC("RPCTransfer", RpcTarget.All, targetId);
    }

    [PunRPC]
    private void RPCTransfer(int id)
    {
        if (id == -1) base.TITransfer(null);
        else
        {
            PhotonView targetPhotonView = PhotonView.Find(id);
            TransferableItemHolder target = targetPhotonView.GetComponent<TransferableItemHolder>();

            base.TITransfer(target);
        }
    }

    protected override void Awake()
    {
        base.Awake();
        photonView = GetComponent<PhotonView>();
    }
}

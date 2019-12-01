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
        photonView.RPC("RPCTransfer", RpcTarget.Others, target.GetComponent<PhotonView>().ViewID);
        base.TITransfer(target);
    }
    [PunRPC]
    private void RPCTransfer(int id)
    {
        TransferableItemHolder target = PhotonView.Find(id).GetComponent<TransferableItemHolder>();
        base.TITransfer(target);
    }


    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }
}

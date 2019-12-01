using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
public class OnlineTransferableItem : TransferableItem 
{
    private PhotonView photonView;

    /// <summary>
    /// El único cambio es que asignamos photonview
    /// </summary>
    protected override void Awake()
    {
        photonView = GetComponent<PhotonView>();
        base.Awake();
    }

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

    /// <summary>
    /// Este es el método que recibe el evento con el id del gameobject del jugador que ha cogido la snitch. A partir de ahí coge el gameobject y ejecuta el código de la clase padre.
    /// </summary>
    [PunRPC]
    private void RPCTransfer(int id)
    {
        TransferableItemHolder target = PhotonView.Find(id).GetComponent<TransferableItemHolder>();
        base.TITransfer(target);
    }
}

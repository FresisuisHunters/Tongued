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
        if (transferActive)
        {
            if (collisionObject.layer == LayerMask.NameToLayer("MainPlayerLayer"))
            {
                TITransfer(collisionObject);
            }
        }
    }

    /// <summary>
    /// Mandamos al resto de jugadores el evento indicando que hemos cogido la snitch con la id de nuestro photonview y luego ejecutamos el código de la clase padre
    /// </summary>
    /// <param name="target"></param>
    protected override void TITransfer(GameObject target)
    {
        photonView.RPC("RPCTransfer", RpcTarget.Others, target.GetPhotonView().ViewID);
        base.TITransfer(target);
    }

    /// <summary>
    /// Este es el método que recibe el evento con el id del gameobject del jugador que ha cogido la snitch. A partir de ahí coge el gameobject y ejecuta el código de la clase padre.
    /// </summary>
    /// <param name="id"></param>
    [PunRPC]
    private void RPCTransfer(int id)
    {
        GameObject target = PhotonView.Find(id).gameObject;
        base.TITransfer(target);
    }
}

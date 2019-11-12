using UnityEngine;
using Photon.Pun;


/// <summary>
/// Hijo de Hook, le añade funcionalidades necesarias para sincronizar Hook.
/// Hace override a las funcione que hay que sincronizar, llamando el RCP al resto de jugadores si es el jugador local.
/// </summary>
[RequireComponent(typeof(PhotonView))]
public class OnlineHook : Hook, IPunObservable, IPunInstantiateMagicCallback
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

    [PunRPC]
    public override void Throw(Vector2 targetPoint)
    {
        //Como RpcTarget es others, si somos el jugador local es que hemos llegado aquí por lógica de juego, no RPC. Mandamos el mensaje al resto.
        if (photonView.IsMine) photonView.RPC("Throw", RpcTarget.Others, targetPoint);

        base.Throw(targetPoint);
    }

    [PunRPC]
    public override void Disable()
    {
        //Como RpcTarget es others, si somos el jugador local es que hemos llegado aquí por lógica de juego, no RPC. Mandamos el mensaje al resto.
        if (photonView.IsMine) photonView.RPC("Disable", RpcTarget.Others);

        base.Disable();
    }

    [PunRPC]
    protected override void Attach(Vector2 attachPoint)
    {
        //Como RpcTarget es others, si somos el jugador local es que hemos llegado aquí por lógica de juego, no RPC. Mandamos el mensaje al resto.
        if (photonView.IsMine) photonView.RPC("Attach", RpcTarget.Others, attachPoint);

        base.Attach(attachPoint);
    }

    protected override void Awake()
    {
        photonView = GetComponent<PhotonView>();
        name = photonView.Owner.NickName;

        base.Awake();
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        //Al instanciarlo, se le pasa el id del PhotonView del jugador.
        //Sin esto, no podríamos registrar Hooks de jugadores remotos a sus HookThrowers.
        //Esto crea una pequeña dependencia circular entre HookThrower y OnlineHook. Si hay forma de evitar que sea así perfecto, pero no se he encontrado ninguna.
        PhotonView throwerView = PhotonView.Find((int) photonView.InstantiationData[0]);

        HookThrower thrower = throwerView.GetComponent<HookThrower>();
        thrower.Hook = this;
        ConnectedBody = thrower.Rigidbody;
    }
}

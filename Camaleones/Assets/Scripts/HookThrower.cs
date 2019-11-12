using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

#pragma warning disable 649
/// <summary>
/// Componente encargado de dar órdenes al gancho.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class HookThrower : MonoBehaviour
{
    [SerializeField] private Hook hookPrefab;
    [SerializeField] private float retractDistancePerSecond = 10f;
    
    public bool HookIsOut => Hook.IsOut;

    public Hook Hook { get; set; }
    public Rigidbody2D Rigidbody { get; private set; }


    public void ThrowHook(Vector2 targetPoint)
    {
        Hook.Throw(targetPoint);
    }

    public void Retract(float time)
    {
        if (Hook.IsAttached) Hook.Length -= retractDistancePerSecond * time;
    }

    public void LetGo()
    {
        Hook.Disable();
    }

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody2D>();

        //Si estamos jugando online (tenemos un PhotonView) y somos el jugador local, utilizamos PhotonNetwork para instanciar el gancho. Si no, un Instantiate de toda la vida.
        //En el prefab  online se asigna OnlineHook, en el prefab offline se asigna Hook.
        Photon.Pun.PhotonView photonView = GetComponent<Photon.Pun.PhotonView>();
        if (!photonView) Hook = Instantiate(hookPrefab);
        else if (photonView.IsMine) Hook = Photon.Pun.PhotonNetwork.Instantiate(hookPrefab.name, Vector3.zero, Quaternion.identity, data: new object[]{ photonView.ViewID}).GetComponent<Hook>();

        if (Hook) Hook.ConnectedBody = Rigidbody;
    }
}

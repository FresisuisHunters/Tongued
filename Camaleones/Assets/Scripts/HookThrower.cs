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
    
    public bool HookIsOut => hook.IsOut;

    private Hook hook = null;
    private new Rigidbody2D rigidbody = null;


    public void ThrowHook(Vector2 targetPoint)
    {
        hook.Throw(rigidbody, targetPoint);
    }

    public void Retract(float time)
    {
        if (hook.IsAttached) hook.Length -= retractDistancePerSecond * time;
    }

    public void LetGo()
    {
        hook.Disable();
    }


    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();

        //Si estamos jugando online (tenemos un PhotonView) utilizamos PhotonNetwork para instanciar el gancho. Si no, un Instantiate de toda la vida.
        //En el prefab  online se asigna OnlineHook, en el prefab offline se asigna Hook.
        if (GetComponent<Photon.Pun.PhotonView>()) hook = Photon.Pun.PhotonNetwork.Instantiate(hookPrefab.name, Vector3.zero, Quaternion.identity).GetComponent<Hook>();
        else hook = Instantiate(hookPrefab);

        hook.gameObject.name = gameObject.name + "'s Hook";

        LetGo();
    }
}

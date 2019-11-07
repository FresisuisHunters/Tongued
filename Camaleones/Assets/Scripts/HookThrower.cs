using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Componente encargado de dar órdenes al gancho.
/// </summary>
#pragma warning disable 649
[RequireComponent(typeof(Rigidbody2D))]
public class HookThrower : MonoBehaviour
{
    [SerializeField] private Hook hookPrefab;
    [SerializeField] private float retractDistancePerSecond = 10f;
    
    public bool HookIsOut => hook.IsOut;

    private Hook hook;
    private new Rigidbody2D rigidbody;


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

        hook = Instantiate(hookPrefab);
        LetGo();
    }
}

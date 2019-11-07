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

        hook = Instantiate(hookPrefab);
        LetGo();
    }
}

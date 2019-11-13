using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookHeadCollisionDetector : MonoBehaviour
{
    private Hook hook;

    // Start is called before the first frame update
    void Awake()
    {
        hook = GetComponentInParent<Hook>();
    }

    /// <summary>
    /// Se redirige la colisión de este objeto al gancho para que la gestione
    /// </summary>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        hook.Collide(collision);
    }
}

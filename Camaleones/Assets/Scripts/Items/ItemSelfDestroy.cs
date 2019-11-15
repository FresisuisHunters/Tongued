using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSelfDestroy : MonoBehaviour
{
    [HideInInspector]public bool active;
    private void Awake()
    {
        active = false;
    }

    /// <summary>
    /// Si el script está activo, destruye el objeto que lo tiene al contacto
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (active && collision.gameObject.layer == LayerMask.NameToLayer("MainPlayerLayer") || collision.gameObject.layer == LayerMask.NameToLayer("PlayerLayer"))
            Destroy(gameObject);
    }
}

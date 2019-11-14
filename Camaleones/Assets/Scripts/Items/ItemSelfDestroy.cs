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
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (active)
            Destroy(gameObject);
    }
}

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Platform"))
            hook.Collide(collision);
    }
}

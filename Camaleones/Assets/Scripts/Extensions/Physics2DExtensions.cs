using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Physics2DExtensions
{
    //Creamos y guardamos arrays de Colliders al principio para no generar basura cada vez que se llama IgnoreCollisions.
    private const int COLLIDER_ARRAY_SIZE = 3;
    private static Collider2D[] aColliders = new Collider2D[COLLIDER_ARRAY_SIZE];
    private static Collider2D[] bColliders = new Collider2D[COLLIDER_ARRAY_SIZE];

    public static void IgnoreCollisions(Rigidbody2D a, Rigidbody2D b, bool ignore)
    {
        int aColliderCount = a.GetAttachedColliders(aColliders);
        int bColliderCount = b.GetAttachedColliders(bColliders);

        if (aColliderCount < a.attachedColliderCount || bColliderCount < b.attachedColliderCount)
        {
            Debug.LogWarning("COLLIDER_ARRAY_SIZE no es suficientemente grande como para recibir todos los Colliders de un Rigidbody que se ha recibido.");
        }

        for (int i = 0; i < aColliderCount; i++)
        {
            for (int j = 0; j < bColliderCount; j++)
            {
                Physics2D.IgnoreCollision(aColliders[i], bColliders[j], ignore);
            }
        }
    }
}
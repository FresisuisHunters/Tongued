using System;
using System.Collections.Generic;
using UnityEngine;

public class RopeCollider : MonoBehaviour
{
    [SerializeField] LayerMask raycastMask;

    [NonSerialized] public Vector2 freeSwingingEndPoint;
    [NonSerialized] public Vector2 fixedEndPoint;

    private RaycastHit2D[] raycastHits = new RaycastHit2D[1];

    public Vector2[] GetRopePoints()
    {
        //TODO: devolver los puntos con los contactos, en ve de solo principio y final. 
        //Idealmente, no crearíamos el array en cada llamada, para evitar generar basura cada frame.
        return new Vector2[] { freeSwingingEndPoint, fixedEndPoint };
    }


    private void FixedUpdate()
    {
        //See if any contact needs to be undone
        //TODO: Eso.

        //Find new contacts
        int hitCount = Physics2D.RaycastNonAlloc(freeSwingingEndPoint, fixedEndPoint, raycastHits, float.MaxValue, raycastMask);
        if (hitCount > 0)
        {
            RaycastHit2D hit = raycastHits[0];
            ContactPoint newContact = new ContactPoint()
            {
                position = hit.point
            };

            //TODO: recordar este contacto en algún sitio.
            //TODO: ver si hay más contactos después de este.
        }
    }
    

    public class ContactPoint
    {
        public Vector2 position;
    }
}

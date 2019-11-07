using System;
using System.Collections.Generic;
using UnityEngine;

public class RopeCollider : MonoBehaviour
{
    [SerializeField] private LayerMask raycastMask;

    [NonSerialized] public Vector2 freeSwingingEndPoint;
    [NonSerialized] public Vector2 fixedEndPoint;

    /// <summary>
    /// Mantiene los puntos de contacto en el orden de su posición en la cuerda. Index 0 es el contacto más cercano al punto fijo.
    /// </summary>
    private List<ContactPoint> contactPoints = new List<ContactPoint>();

    private RaycastHit2D[] raycastHits = new RaycastHit2D[1];


    public Vector2[] GetRopePoints()
    {
        //TODO: devolver los puntos con los contactos, en ve de solo principio y final. 
        //Idealmente, no crearíamos el array en cada llamada, para evitar generar basura cada frame.
        //return new Vector2[] { freeSwingingEndPoint, fixedEndPoint };

        Vector2[] points = new Vector2[contactPoints.Count + 2];
        for (int i = 0; i < contactPoints.Count; i++)
        {
            points[i + 1] = contactPoints[i].position;
        }

        points[0] = fixedEndPoint;
        points[points.Length - 1] = freeSwingingEndPoint;

        return points;

    }

    public void ClearContacts()
    {
        contactPoints.Clear();
    }


    private void FixedUpdate()
    {
        DetectUndoneContacts();
        DetectNewContacts();
    }


    private void DetectNewContacts()
    {
        Vector2 a = freeSwingingEndPoint; 
        Vector2 b = contactPoints.Count > 0 ? contactPoints[contactPoints.Count - 1].position : fixedEndPoint;

        if (FindContactPointInSegment(a, b, out ContactPoint contactPoint))
        {
            contactPoints.Add(contactPoint);
        }
    }

    private bool FindContactPointInSegment(Vector2 a, Vector2 b, out ContactPoint contactPoint)
    { 
        int hitCount = Physics2D.RaycastNonAlloc(a, b - a, raycastHits, Vector2.Distance(a, b), raycastMask);
        
        if (hitCount > 0)
        {
            RaycastHit2D hit = raycastHits[0];

            if (hit.collider is PolygonCollider2D collider)
            {
                Vector2 position = GetClosestVertex(hit.point, collider);
                if (!PositionIsAlreadyAContactPoint(position))
                {
                    contactPoint = new ContactPoint()
                    {
                        position = position,
                        angleSign = Mathf.Sign(Vector2.SignedAngle(a - position, b - position))
                    };
                    return true;
                }
            }
        }

        contactPoint = new ContactPoint();
        return false;
    }

    private bool PositionIsAlreadyAContactPoint(Vector2 position)
    {
        for (int i = 0; i < contactPoints.Count; i++)
        {
            if (contactPoints[i].position == position) return true;
        }

        return false;
    }

    private Vector2 GetClosestVertex(Vector2 point, PolygonCollider2D collider)
    {
        Vector2[] vertices = collider.points;
        Transform transform = collider.transform;

        float minDistance = float.MaxValue;
        float distance;
        Vector2 closestPoint = Vector2.zero;
        Vector2 vertexInWorldSpace;
        for (int i = 0; i < vertices.Length; i++)
        {
            vertexInWorldSpace = transform.TransformPoint(vertices[i]);
            distance = (point - vertexInWorldSpace).sqrMagnitude;
            if (distance < minDistance)
            {
                minDistance = distance;
                closestPoint = vertexInWorldSpace;
            }
        }

        return closestPoint;
    }


    private void DetectUndoneContacts()
    {
        if (contactPoints.Count > 0)
        {
            Vector2 a = freeSwingingEndPoint;
            Vector2 b = contactPoints.Count > 1 ? contactPoints[contactPoints.Count - 2].position : fixedEndPoint;

            ContactPoint mostRecentContact = contactPoints[contactPoints.Count - 1];
            if (ShouldUndoContact(a, mostRecentContact, b))
            {
                contactPoints.RemoveAt(contactPoints.Count - 1);
                print("undone");
            }
        }
    }

    private bool ShouldUndoContact(Vector2 previousPoint, ContactPoint contactPoint, Vector2 nextPoint)
    {
        float currentAngleSign = Mathf.Sign(Vector2.SignedAngle(previousPoint - contactPoint.position, nextPoint - contactPoint.position));
        return currentAngleSign != contactPoint.angleSign;
    }
    

    public struct ContactPoint
    {
        public Vector2 position;
        public float angleSign;
    }
}

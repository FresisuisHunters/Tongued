using System;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 649
/// <summary>
/// Componente que se encarga de detectar los puntos de contacto de la cuerda del gancho, 
/// y de hacer que estos afecten a su comportamiento.
/// </summary>
/// Limitaciones: 
/// - Sólo encuentra puntos de contacto en PolygonCollider2Ds, ya que son los únicos de los que podemos sacar una lista de vértices.
/// - Los puntos de contacto son estáticos - no se pueden mover.
/// - Generamos un array para devolver los puntos cada vez que se pide - eso genera mucha basura.
[RequireComponent(typeof(DistanceJoint2D))]
public class RopeCollider : MonoBehaviour
{
    #region Inspector
    [SerializeField, Tooltip("Las layers en las que se buscan puntos de contacto.")]
    private LayerMask raycastMask;
    #endregion

    #region Public members
    /// <summary>
    /// La posición de la cabeza del gancho.
    /// </summary>
    public Vector2 HeadPosition
    {
        get => _headPosition;
        set
        {
            _headPosition = value;
            if (contactPoints.Count == 0) UpdateSwingingPoint();
        }
    }
    private Vector2 _headPosition;

    public float SwingingSegmentLength {
        get
        {
            if (contactPoints.Count > 0)
            {
                return (contactPoints[contactPoints.Count - 1].position - freeSwingingEndPoint).magnitude;
            }
            else
            {
                return (_headPosition - freeSwingingEndPoint).magnitude;
            }
            
        }
    }

    /// <summary>
    /// La posición del final de la cuerda que cuelga libremente.
    /// </summary>
    [NonSerialized] public Vector2 freeSwingingEndPoint;
    #endregion

    public event Action OnClearedContacts;

    #region Private State
    /// <summary>
    /// Mantiene los puntos de contacto en el orden de su posición en la cuerda. Index 0 es el contacto más cercano al punto fijo.
    /// </summary>
    private List<ContactPoint> contactPoints = new List<ContactPoint>();
    /// <summary>
    /// Array cacheado para no generar basura al hacer raycasts.
    /// </summary>
    private RaycastHit2D[] raycastHits = new RaycastHit2D[1];

    private DistanceJoint2D distanceJoint;
    private Rigidbody2D swingHingeRigidbody;

    #endregion

    /// <summary>
    /// Index 0: punto fijo
    /// Último index: swinger
    /// </summary>
    /// <returns></returns>
    public Vector3[] GetRopePoints()
    {
        //Idealmente, no crearíamos el array en cada llamada, para evitar generar basura cada frame.
        Vector3[] points = new Vector3[contactPoints.Count + 2];
        for (int i = 0; i < contactPoints.Count; i++)
        {
            points[i + 1] = contactPoints[i].position;
        }

        points[0] = HeadPosition;
        points[points.Length - 1] = freeSwingingEndPoint;

        return points;
    }

    /// <summary>
    /// Vacía la lista de contactos.
    /// </summary>
    public void ClearContacts()
    {
        contactPoints.Clear();
        OnClearedContacts?.Invoke();
    }


    private void FixedUpdate()
    {
        DetectUndoneContacts();
        DetectNewContacts();
    }

    #region Deshacer contactos
    private void DetectUndoneContacts()
    {
        //Si no hay puntos de contacto, no hay nada que deshacer
        if (contactPoints.Count > 0)
        {
            //Miramos si el punto de contacto más cercano al final que cuelga se ha deshecho.
            Vector2 a = freeSwingingEndPoint;
            Vector2 b = contactPoints.Count > 1 ? contactPoints[contactPoints.Count - 2].position : HeadPosition;

            ContactPoint mostRecentContact = contactPoints[contactPoints.Count - 1];
            if (ShouldUndoContact(a, mostRecentContact, b))
            {
                //Devolvemos la longitud de este punto de contacto al DistanceJoint2D
                distanceJoint.distance += mostRecentContact.length;
                contactPoints.RemoveAt(contactPoints.Count - 1);
                UpdateSwingingPoint();
            }
        }
    }

    private bool ShouldUndoContact(Vector2 previousPoint, ContactPoint contactPoint, Vector2 nextPoint)
    {
        //Un segmento está deshecho cuando el signo del ángulo entre los segmentos que separa deja de ser el mismo que cuando se creó.
        float currentAngleSign = Mathf.Sign(Vector2.SignedAngle(previousPoint - contactPoint.position, nextPoint - contactPoint.position));
        return currentAngleSign != contactPoint.angleSign;
    }
    #endregion

    #region Encontrar contactos
    private void DetectNewContacts()
    {
        //Buscamos contactos entre el final que cuelga y el punto del que cuelga
        Vector2 a = freeSwingingEndPoint;
        Vector2 b = swingHingeRigidbody.position;

        if (FindContactPointInSegment(a, b, out ContactPoint contactPoint))
        {
            //Quitamos la longitud del punto de contacto al DistanceJoint
            contactPoints.Add(contactPoint);
            distanceJoint.distance -= contactPoint.length;
            UpdateSwingingPoint();
        }
    }

    private bool FindContactPointInSegment(Vector2 a, Vector2 b, out ContactPoint contactPoint)
    {
        //Haceos un raycast entre los dos extremos del segmento
        int hitCount = Physics2D.RaycastNonAlloc(a, b - a, raycastHits, Vector2.Distance(a, b), raycastMask);

        if (hitCount > 0)
        {
            RaycastHit2D hit = raycastHits[0];

            //Sólo nos importa si es un PolygonCollider2D
            if (hit.collider is PolygonCollider2D collider)
            {
                //Creamos el punto de contacto en el vértice más cercano a la colisión
                Vector2 position = GetClosestVertex(hit.point, collider);
                if (!IsDuplicateContactPoint(position))
                {
                    contactPoint = new ContactPoint()
                    {
                        position = position,
                        angleSign = Mathf.Sign(Vector2.SignedAngle(a - position, b - position)),
                        length = Vector2.Distance(b, position)
                    };
                    return true;
                }
            }
        }

        contactPoint = new ContactPoint();
        return false;
    }

    private bool IsDuplicateContactPoint(Vector2 position)
    {
        if (contactPoints.Count > 0)
        {
            return contactPoints[contactPoints.Count - 1].position == position;
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
    #endregion

    private void UpdateSwingingPoint()
    {
        Vector2 hingePoint = (contactPoints.Count > 0) ? contactPoints[contactPoints.Count - 1].position : HeadPosition;
        swingHingeRigidbody.position = hingePoint;
    }

    #region Initialization
    private void Awake()
    {
        distanceJoint = GetComponent<DistanceJoint2D>();

        swingHingeRigidbody = GetComponent<Rigidbody2D>();
        swingHingeRigidbody.isKinematic = true;
    }
    #endregion



    public struct ContactPoint
    {
        public Vector2 position;
        public float angleSign;
        public float length;
    }
}

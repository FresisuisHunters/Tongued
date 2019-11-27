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
[RequireComponent (typeof (DistanceJoint2D))]
public class RopeCollider : MonoBehaviour {
    #region Inspector
    [SerializeField, Tooltip ("Las layers en las que se buscan puntos de contacto.")]
    private LayerMask raycastMask;
    #endregion

    #region Public members
    /// <summary>
    /// La posición de la cabeza del gancho.
    /// </summary>
    public Vector2 HeadPosition {
        get => _headPosition;
        set {
            _headPosition = value;
            if (contactPoints.Count == 0) UpdateSwingingPoint ();
        }
    }
    private Vector2 _headPosition;

    public Vector2 SwingingHingePoint {
        get {
            if (contactPoints.Count > 0) {
                return contactPoints[contactPoints.Count - 1].position;
            } else {
                return _headPosition;
            }
        }
    }
    public float SwingingSegmentLength {
        get {
            return (SwingingHingePoint - freeSwingingEndPoint).magnitude;
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
    private List<ContactPoint> contactPoints = new List<ContactPoint> ();
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
    public Vector3[] GetRopePoints () {
        //Idealmente, no crearíamos el array en cada llamada, para evitar generar basura cada frame.
        Vector3[] points = new Vector3[contactPoints.Count + 2];
        for (int i = 0; i < contactPoints.Count; i++) {
            points[i + 1] = contactPoints[i].position;
        }

        points[0] = HeadPosition;
        points[points.Length - 1] = freeSwingingEndPoint;

        return points;
    }

    /// <summary>
    /// Vacía la lista de contactos.
    /// </summary>
    public void ClearContacts () {
        contactPoints.Clear ();
        OnClearedContacts?.Invoke ();
    }

    private void FixedUpdate () {
        DetectUndoneContacts ();
        DetectNewContacts ();
    }

    #region Deshacer contactos
    private void DetectUndoneContacts () {
        //Si no hay puntos de contacto, no hay nada que deshacer
        if (contactPoints.Count == 0) {
            return;
        }

        for (int i = 0; i < contactPoints.Count; ++i) {
            Vector2 a = (i == 0) ? freeSwingingEndPoint : contactPoints[contactPoints.Count - i].position;
            Vector2 b = (i == contactPoints.Count - 1) ? HeadPosition : contactPoints[contactPoints.Count - (i + 2)].position;

            ContactPoint contactPoint = contactPoints[contactPoints.Count - (i + 1)];
            if (ShouldUndoContact (a, contactPoint, b)) {
                bool isContactPointNextToHead = contactPoints.IndexOf (contactPoint) == contactPoints.Count - 1;
                if (isContactPointNextToHead) {
                    distanceJoint.distance += contactPoint.length;
                } else {
                    ContactPoint previousContactPoint = contactPoints[contactPoints.Count - i];
                    previousContactPoint.length += contactPoint.length;
                }

                contactPoints.RemoveAt (contactPoints.Count - (i + 1));

                if (isContactPointNextToHead) {
                    UpdateSwingingPoint ();
                }
            }
        }
    }

    private bool ShouldUndoContact (Vector2 previousPoint, ContactPoint contactPoint, Vector2 nextPoint) {
        //Un segmento está deshecho cuando el signo del ángulo entre los segmentos que separa deja de ser el mismo que cuando se creó.
        float currentAngleSign = Mathf.Sign (Vector2.SignedAngle (previousPoint - contactPoint.position, nextPoint - contactPoint.position));
        return currentAngleSign != contactPoint.angleSign;
    }
    #endregion

    #region Encontrar contactos
    private void DetectNewContacts () {
        /*
        for (int i = 0; i <= contactPoints.Count; ++i) {
            Vector2 a = (i == 0) ? freeSwingingEndPoint : contactPoints[contactPoints.Count - i].position;
            Vector2 b = (i == contactPoints.Count) ? HeadPosition : contactPoints[contactPoints.Count - (i + 1)].position;

            if (FindContactPointInSegment (a, b, out ContactPoint contactPoint)) {
                bool isContactPointNextToHead = contactPoints.IndexOf (contactPoint) == contactPoints.Count - 1;
                if (isContactPointNextToHead) {
                    distanceJoint.distance -= contactPoint.length;
                } else {
                    ContactPoint previousContactPoint = contactPoints[contactPoints.Count - i];
                    previousContactPoint.length -= contactPoint.length;
                    // Actualizar el siguiente?
                }

                contactPoints.Insert (contactPoints.Count - i, contactPoint);

                if (isContactPointNextToHead) {
                    UpdateSwingingPoint ();
                }
            }
        }
        */

        //Buscamos contactos entre el final que cuelga y el punto del que cuelga
        Vector2 a = freeSwingingEndPoint;
        Vector2 b = swingHingeRigidbody.position;

        if (FindContactPointInSegment (a, b, out ContactPoint contactPoint)) {
            //Quitamos la longitud del punto de contacto al DistanceJoint
            contactPoints.Add (contactPoint);
            distanceJoint.distance -= contactPoint.length;

            UpdateSwingingPoint ();
        }

        // Buscamos contactos entre el extremo de la lengua y el punto de contacto previo
        if (contactPoints.Count == 0) {
            // El caso en que no hubiese puntos de contacto los detecta el punto anterior
            return;
        }

        Vector2 previousContactPointToTongueEndPosition = contactPoints[0].position;
        Vector2 tongueEndPosition = HeadPosition;
        Vector2 tongueDirection = (tongueEndPosition - previousContactPointToTongueEndPosition).normalized;
        Vector2 contactPointOffset = previousContactPointToTongueEndPosition + tongueDirection;
        if (FindContactPointInSegment (tongueEndPosition, contactPointOffset,
                out ContactPoint otherContactPoint)) {
            contactPoints.Insert (0, otherContactPoint);
            
            Debug.Log(contactPoints.Count);
        }
    }

    private bool FindContactPointInSegment (Vector2 a, Vector2 b, out ContactPoint contactPoint) {
        //Hacemos un raycast entre los dos extremos del segmento
        int hitCount = Physics2D.RaycastNonAlloc (a, b - a, raycastHits, Vector2.Distance (a, b), raycastMask);

        if (hitCount > 0) {
            RaycastHit2D hit = raycastHits[0];

            //Sólo nos importa si es un PolygonCollider2D
            if (hit.collider is PolygonCollider2D collider) {
                //Creamos el punto de contacto en el vértice más cercano a la colisión
                Vector2 position = GetClosestVertex (hit.point, collider);
                if (!IsDuplicateContactPoint (position)) {
                    contactPoint = new ContactPoint () {
                        position = position,
                        angleSign = Mathf.Sign (Vector2.SignedAngle (a - position, b - position)),
                        length = Vector2.Distance (b, position)
                    };
                    return true;
                }
            }
        }

        contactPoint = new ContactPoint ();
        return false;
    }

    private bool IsDuplicateContactPoint (Vector2 position) {
        if (contactPoints.Count > 0) {
            return contactPoints[contactPoints.Count - 1].position == position;
        }

        return false;
    }

    private Vector2 GetClosestVertex (Vector2 point, PolygonCollider2D collider) {
        Vector2[] vertices = collider.points;
        Transform transform = collider.transform;

        float minDistance = float.MaxValue;
        float distance;
        Vector2 closestPoint = Vector2.zero;
        Vector2 vertexInWorldSpace;
        for (int i = 0; i < vertices.Length; i++) {
            vertexInWorldSpace = transform.TransformPoint (vertices[i]);
            distance = (point - vertexInWorldSpace).sqrMagnitude;
            if (distance < minDistance) {
                minDistance = distance;
                closestPoint = vertexInWorldSpace;
            }
        }

        return closestPoint;
    }

    #endregion

    private void UpdateSwingingPoint () {
        Vector2 hingePoint = (contactPoints.Count > 0) ? contactPoints[contactPoints.Count - 1].position : HeadPosition;
        swingHingeRigidbody.position = hingePoint;
    }

    #region Initialization
    private void Awake () {
        distanceJoint = GetComponent<DistanceJoint2D> ();

        swingHingeRigidbody = GetComponent<Rigidbody2D> ();
        swingHingeRigidbody.isKinematic = true;
    }
    #endregion

    public struct ContactPoint {
        public Vector2 position;
        public float angleSign;
        public float length;
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DistanceJoint2D))]
public class Hook : MonoBehaviour
{
    [Tooltip("La magnitud de la fuerza que se le aplica al cuerpo conectado cuando el gancho se engancha a algo.")]
    public float forceOnAttached = 10;
    [Tooltip("La fuerza al enganchar sólo se aplica si el cuerpo conectado va a menos que esta velocidad.")]
    public float maxVelocityForForceOnAttached = 20;

    public bool IsOut => isActiveAndEnabled;
    public bool IsAttached { get; private set; }

    /// <summary>
    /// La longitud máxima del gancho.
    /// </summary>
    public float Length { get => distanceJoint.distance; set => distanceJoint.distance = value; }
    

    private DistanceJoint2D distanceJoint;
    private new Rigidbody2D rigidbody;
    private RopeCollider ropeCollider;


    public void Throw(Rigidbody2D connectedBody, Vector2 targetPoint)
    {
        gameObject.SetActive(true);
        IsAttached = false;

        rigidbody.position = targetPoint;
        distanceJoint.connectedBody = connectedBody;

        Attach();        
    }

    public void Disable()
    {
        gameObject.SetActive(false);
        IsAttached = false;
    }

    //Ahora mismo se llama a la que se echa el gancho, pero más tarde el gancho será un proyectil y Attach() se llamará una vez choque con algo.
    private void Attach()
    {
        Rigidbody2D connectedBody = distanceJoint.connectedBody;

        distanceJoint.distance = Vector2.Distance(rigidbody.position, connectedBody.position);
        distanceJoint.enabled = true;

        if (connectedBody.velocity.magnitude < maxVelocityForForceOnAttached)
        {
            Vector2 attachForceOnThrower = (rigidbody.position - connectedBody.position).normalized * forceOnAttached;
            connectedBody.AddForce(attachForceOnThrower);
        }
        
        IsAttached = true;
    }

    private void FixedUpdate()
    {
        //Actualiza las posiciones finales de RopeCollider.
        ropeCollider.freeSwingingEndPoint = distanceJoint.connectedBody.position;
        ropeCollider.fixedEndPoint = rigidbody.position;
    }

    //Visualización provisional
    private void OnDrawGizmos()
    {
        if (Application.isPlaying && distanceJoint.connectedBody)
        {
            Vector2[] ropePoints = ropeCollider.GetRopePoints();

            for (int i = 0; i < ropePoints.Length - 1; i++)
            {
                Gizmos.DrawLine(ropePoints[i], ropePoints[i + 1]);
            } 
        }
    }


    private void Awake()
    {
        enabled = true; //Al no tener Start ni Update, enabled==false por defecto. Lo ponemos a true para que HookThrower sepa si el gancho está activo.

        distanceJoint = GetComponent<DistanceJoint2D>();
        distanceJoint.enableCollision = false;
        distanceJoint.maxDistanceOnly = true;
        distanceJoint.autoConfigureDistance = false;
        distanceJoint.autoConfigureConnectedAnchor = false;

        rigidbody = GetComponent<Rigidbody2D>();
        ropeCollider = GetComponentInChildren<RopeCollider>();
    }
}

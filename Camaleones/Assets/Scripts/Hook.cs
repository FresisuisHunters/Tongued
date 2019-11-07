using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DistanceJoint2D))]
public class Hook : MonoBehaviour
{
    [Tooltip("La magnitud de la fuerza que se le aplica al cuerpo conectado cuando el gancho se engancha a algo.")]
    public float forceOnAttached = 10;

    public bool IsOut => isActiveAndEnabled;
    public bool IsAttached { get; private set; }

    /// <summary>
    /// La longitud máxima del gancho.
    /// </summary>
    public float Length { get => distanceJoint.distance; set => distanceJoint.distance = value; }
    

    private DistanceJoint2D distanceJoint;
    private new Rigidbody2D rigidbody;


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


    private void Attach()
    {
        distanceJoint.distance = Vector2.Distance(rigidbody.position, distanceJoint.attachedRigidbody.position);
        distanceJoint.enabled = true;

        Vector2 initialForceOnThrower = (rigidbody.position - distanceJoint.attachedRigidbody.position).normalized * forceOnAttached;
        distanceJoint.attachedRigidbody.AddForce(initialForceOnThrower);

        IsAttached = true;
    }

    //Visualización provisional
    private void OnDrawGizmos()
    {
        if (Application.isPlaying && distanceJoint.connectedBody) Gizmos.DrawLine(distanceJoint.connectedBody.position, rigidbody.position);
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
    }
}

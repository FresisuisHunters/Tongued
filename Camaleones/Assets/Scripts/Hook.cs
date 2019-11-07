using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DistanceJoint2D))]
public class Hook : MonoBehaviour
{
    public float forceOnAttached = 10;

    public float Length { get => distanceJoint.distance; set => distanceJoint.distance = value; }

    private DistanceJoint2D distanceJoint;
    private new Rigidbody2D rigidbody;

    public void Throw(HookThrower thrower, Vector2 targetPoint)
    {
        gameObject.SetActive(true);

        rigidbody.position = targetPoint;
        Vector2 throwerPos = thrower.Rigidbody.position;

        distanceJoint.connectedBody = thrower.Rigidbody;
        distanceJoint.distance = Vector2.Distance(targetPoint, throwerPos);
        distanceJoint.enabled = true;


    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }


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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpringJoint2D))]
public class Hook : MonoBehaviour
{
    public float forceOnAttached = 10;

    public float Length { get => springJoint.distance; set => springJoint.distance = value; }

    private SpringJoint2D springJoint;
    private new Rigidbody2D rigidbody;

    public void Throw(HookThrower thrower, Vector2 targetPoint)
    {
        gameObject.SetActive(true);

        rigidbody.position = targetPoint;
        Vector2 throwerPos = thrower.Rigidbody.position;

        springJoint.connectedBody = thrower.Rigidbody;
        springJoint.distance = Vector2.Distance(targetPoint, throwerPos);
        springJoint.enabled = true;


    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }


    private void OnDrawGizmos()
    {
        if (Application.isPlaying && springJoint.connectedBody) Gizmos.DrawLine(springJoint.connectedBody.position, rigidbody.position);
    }


    private void Awake()
    {
        enabled = true; //Al no tener Start ni Update, enabled==false por defecto. Lo ponemos a true para que HookThrower sepa si el gancho está activo.

        springJoint = GetComponent<SpringJoint2D>();
        springJoint.enableCollision = false;
        springJoint.autoConfigureDistance = false;
        springJoint.autoConfigureConnectedAnchor = false;

        rigidbody = GetComponent<Rigidbody2D>();
    }
}

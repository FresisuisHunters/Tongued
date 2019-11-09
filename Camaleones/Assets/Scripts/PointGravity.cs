using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointGravity : MonoBehaviour
{
    public float gravityStrength = 1000;
    public float distanceScale = 1;

    private new Transform transform;
    private List<Rigidbody2D> affectedBodies = new List<Rigidbody2D>();


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.attachedRigidbody) affectedBodies.Remove(collision.attachedRigidbody);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.attachedRigidbody) affectedBodies.Add(collision.attachedRigidbody);
    }

    private void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;
        Vector2 u;
        float f;
        float r;

        for (int i = 0; i < affectedBodies.Count; i++)
        {
            u = (Vector2) transform.position - affectedBodies[i].position;
            r = u.magnitude * distanceScale;
            u.Normalize();

            f = (gravityStrength * affectedBodies[i].mass) / (r * r);
            affectedBodies[i].AddForce(u * f * dt);
        }
    }

    private void Awake()
    {
        Rigidbody2D[] bodies = FindObjectsOfType<Rigidbody2D>();

        for (int i = 0; i < bodies.Length; i++)
        {
            if (bodies[i].bodyType == RigidbodyType2D.Dynamic)
            {
                affectedBodies.Add(bodies[i]);
            }
        }

        transform = GetComponent<Transform>();

        Physics2D.gravity = Vector2.zero;

    }
}

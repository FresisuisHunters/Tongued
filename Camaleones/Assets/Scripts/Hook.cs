using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Componente central del gancho. Maneja activarlo, engancharlo y desactivarlo; 
/// y da la información necesaria a los otros componentes del gancho.
/// El gancho está dividido en dos partes: la cabeza (la parte que se engancha) 
/// y el swinging point (el punto del que cuelga el jugador, que puede moverse en algunos casos.
/// </summary>
public class Hook : MonoBehaviour
{
    #region Inspector
    [Tooltip("La magnitud de la fuerza que se le aplica al cuerpo conectado cuando el gancho se engancha a algo.")]
    public float forceOnAttached = 10;
    [Tooltip("La fuerza al enganchar sólo se aplica si el cuerpo conectado va a menos que esta velocidad.")]
    public float maxVelocityForForceOnAttached = 20;

    [SerializeField] private Rigidbody2D headRigidbody;
    #endregion

    #region Propiedades Públicas
    public bool IsOut => isActiveAndEnabled;
    public bool IsAttached { get; private set; }
    /// <summary>
    /// La longitud de cuerda de la que cuelga el cuerpo conectado
    /// </summary>
    public float Length { get => distanceJoint.distance; set => distanceJoint.distance = value; }
    #endregion

    private DistanceJoint2D distanceJoint;
    private RopeCollider ropeCollider;
    [Tooltip("Velocidad del proyectil con el que se engancha el personaje")]
    [SerializeField] private float hookProjectileSpeed;
    [Tooltip("Rango maximo del gancho al ser lanzado")]
    [Range(0, 50)]
    [SerializeField] private float maxHookDistance;
    private bool isBeingThrown;
    private Vector2 throwDirection;

    /// <summary>
    /// Metodo que se llama cuando se lanza el gancho.
    /// Recibe el punto hacia el cual se lanza el gancho y el RigidBody del personaje que lo lanza
    /// </summary>
    /// <param name="connectedBody"></param>
    /// <param name="targetPoint"></param>
    public void Throw(Rigidbody2D connectedBody, Vector2 targetPoint)
    {
        gameObject.SetActive(true);
        IsAttached = false;
        distanceJoint.connectedBody = connectedBody;
        Vector2 startPosition = distanceJoint.connectedBody.GetComponent<Transform>().position;
        throwDirection = (targetPoint - startPosition).normalized;
        headRigidbody.transform.position = startPosition;
        isBeingThrown = true;
    }

    /// <summary>
    /// Método que se llama cuando el gancho se desactiva, ya sea cuando es instanciado o cuando se suelta.
    /// </summary>
    public void Disable()
    {
        gameObject.SetActive(false);
        IsAttached = false;

        ropeCollider.ClearContacts();
        ropeCollider.enabled = false;
        ropeCollider.GetComponent<DistanceJoint2D>().enabled = false;
    }

    /// <summary>
    /// Método que se llama cuando la cabeza del gancho entra en contacto con una plataforma tras ser lanzada.
    /// </summary>
    private void Attach()
    {
        Rigidbody2D connectedBody = distanceJoint.connectedBody;
   
        if (connectedBody.velocity.magnitude < maxVelocityForForceOnAttached)
        {
            Vector2 attachForceOnThrower = (headRigidbody.position - connectedBody.position).normalized * forceOnAttached;
            connectedBody.AddForce(attachForceOnThrower);
        }

        distanceJoint.distance = Vector2.Distance(headRigidbody.position, connectedBody.position);

        distanceJoint.enabled = true;
        ropeCollider.enabled = true;
        ropeCollider.GetComponent<DistanceJoint2D>().enabled = true;

        IsAttached = true;
    }

    private void FixedUpdate()
    {
        //Actualiza las posiciones finales de RopeCollider.
        ropeCollider.freeSwingingEndPoint = distanceJoint.connectedBody.position;
        if (!isBeingThrown)
        {
            ropeCollider.HeadPosition = headRigidbody.position;
        }
    }


    //Visualización provisional
    private void OnDrawGizmos()
    {
        if (Application.isPlaying && distanceJoint.connectedBody)
        {
            Vector2[] ropePoints = ropeCollider.GetRopePoints();

            Gizmos.color = Color.white;
            for (int i = 0; i < ropePoints.Length - 1; i++)
            {
                Gizmos.DrawLine(ropePoints[i], ropePoints[i + 1]);
            }
        }
    }


    private void Awake()
    {
        enabled = true; //Al no tener Start ni Update, enabled==false por defecto. Lo ponemos a true para que HookThrower sepa si el gancho está activo.

        distanceJoint = GetComponentInChildren<DistanceJoint2D>();
        distanceJoint.enableCollision = false;
        distanceJoint.maxDistanceOnly = true;
        distanceJoint.autoConfigureDistance = false;
        distanceJoint.autoConfigureConnectedAnchor = false;
        distanceJoint.enabled = false;

        ropeCollider = GetComponentInChildren<RopeCollider>();

        isBeingThrown = false;
    }

    void Update()
    {
        if (isBeingThrown)
        {
            headRigidbody.transform.Translate(throwDirection * hookProjectileSpeed * Time.deltaTime);
            if ((headRigidbody.position - (Vector2)transform.position).magnitude >= maxHookDistance)
            {
                isBeingThrown = false;
            }
        }
    }

    public void Collide(Collider2D collision)
    {
        if(isBeingThrown)
        {
            if (collision.CompareTag("Platform"))
            {
                Attach();
                isBeingThrown = false;
            }
        }
    }

}

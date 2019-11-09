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


    public void Throw(Rigidbody2D connectedBody, Vector2 targetPoint)
    {
        gameObject.SetActive(true);
        IsAttached = false;

        headRigidbody.position = targetPoint;
        distanceJoint.connectedBody = connectedBody;

        Attach();        
    }

    public void Disable()
    {
        gameObject.SetActive(false);
        IsAttached = false;

        ropeCollider.ClearContacts();
    }


    //Ahora mismo se llama a la que se echa el gancho, pero más tarde el gancho será un proyectil y Attach() se llamará una vez choque con algo.
    private void Attach()
    {
        Rigidbody2D connectedBody = distanceJoint.connectedBody;

        distanceJoint.distance = Vector2.Distance(headRigidbody.position, connectedBody.position);
        distanceJoint.enabled = true;

        //Fuerza de enganche. Sólo se aplica si el cuerpo conectado va a menos que cierta velocidad.
        if (connectedBody.velocity.magnitude < maxVelocityForForceOnAttached)
        {
            Vector2 attachForceOnThrower = (headRigidbody.position - connectedBody.position).normalized * forceOnAttached;
            connectedBody.AddForce(attachForceOnThrower);
        }
        
        IsAttached = true;
    }

    protected void FixedUpdate()
    {
        //Actualiza las posiciones finales de RopeCollider.
        ropeCollider.freeSwingingEndPoint = distanceJoint.connectedBody.position;
        ropeCollider.HeadPosition = headRigidbody.position;
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


    protected void Awake()
    {
        enabled = true; //Al no tener Start ni Update, enabled==false por defecto. Lo ponemos a true para que HookThrower sepa si el gancho está activo.

        distanceJoint = GetComponentInChildren<DistanceJoint2D>();
        distanceJoint.enableCollision = false;
        distanceJoint.maxDistanceOnly = true;
        distanceJoint.autoConfigureDistance = false;
        distanceJoint.autoConfigureConnectedAnchor = false;

        ropeCollider = GetComponentInChildren<RopeCollider>();
    }
}

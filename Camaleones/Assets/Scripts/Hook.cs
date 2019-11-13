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
    [Header("Projectil")]
    [Tooltip("Velocidad del proyectil con el que se engancha el personaje")]
    public float hookProjectileSpeed;
    [Tooltip("Rango maximo del gancho al ser lanzado")]
    public float maxHookDistance;

    [Header("Fuerza al enganchar")]
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
    public Rigidbody2D ConnectedBody { get => distanceJoint.connectedBody;
        set
        {
            distanceJoint.connectedBody = value;
            connectedBodyTransform = value.GetComponent<Transform>();
        }
    }
    #endregion

    #region References
    protected DistanceJoint2D distanceJoint;
    protected RopeCollider ropeCollider;
    protected LineRenderer lineRenderer;
    private Transform connectedBodyTransform;
    #endregion

    private bool isBeingThrown;
    private Vector2 throwOriginPoint;

    /// <summary>
    /// Metodo que se llama cuando se lanza el gancho.
    /// Recibe el punto hacia el cual se lanza el gancho y el RigidBody del personaje que lo lanza
    /// </summary>
    public virtual void Throw(Vector2 targetPoint)
    {
        gameObject.SetActive(true);
        IsAttached = false;

        throwOriginPoint = ConnectedBody.position;
        Vector2 throwDirection = (targetPoint - throwOriginPoint).normalized;

        headRigidbody.position = throwOriginPoint;
        headRigidbody.velocity = throwDirection * hookProjectileSpeed;
        //headRigidbody.transform.position = throwOriginPoint;
        headRigidbody.isKinematic = false;
        isBeingThrown = true;
    }

    /// <summary>
    /// Método que se llama cuando el gancho se desactiva, ya sea cuando es instanciado o cuando se suelta.
    /// </summary>
    public virtual void Disable()
    {
        gameObject.SetActive(false);
        IsAttached = false;
        isBeingThrown = false;

        ropeCollider.ClearContacts();
        ropeCollider.enabled = false;

        distanceJoint.enabled = false;

        headRigidbody.isKinematic = true;
    }

    /// <summary>
    /// Método que se llama cuando la cabeza del gancho entra en contacto con una plataforma tras ser lanzada.
    /// </summary>
    protected virtual void Attach(Vector2 attachPoint)
    {
        if (ConnectedBody.velocity.magnitude < maxVelocityForForceOnAttached)
        {
            Vector2 attachForceOnThrower = (attachPoint - ConnectedBody.position).normalized * forceOnAttached;
            ConnectedBody.AddForce(attachForceOnThrower, ForceMode2D.Impulse);
        }

        headRigidbody.position = attachPoint;
        headRigidbody.isKinematic = true;

        //Consider RopeCollider here.
        distanceJoint.distance = Vector2.Distance(attachPoint, ConnectedBody.position);
        distanceJoint.enabled = true;

        ropeCollider.enabled = true;

        IsAttached = true;
    }

    private void FixedUpdate()
    {
        //Actualiza las posiciones finales de RopeCollider.
        ropeCollider.freeSwingingEndPoint = distanceJoint.connectedBody.position;
        ropeCollider.HeadPosition = headRigidbody.position;

        if (isBeingThrown && (headRigidbody.position - throwOriginPoint).magnitude >= maxHookDistance)
        {
            Disable();
        }
    }


    //Visualización provisional
    private void OnDrawGizmos()
    {
        Vector3[] ropePoints = ropeCollider.GetRopePoints();
        ropePoints[ropePoints.Length - 1] = connectedBodyTransform.position;

        lineRenderer.positionCount = ropePoints.Length;
        lineRenderer.SetPositions(ropePoints);
    }


    protected virtual void Awake()
    {
        enabled = true; //Al no tener Start ni Update, enabled==false por defecto. Lo ponemos a true para que HookThrower sepa si el gancho está activo.

        distanceJoint = GetComponentInChildren<DistanceJoint2D>();
        distanceJoint.enableCollision = false;
        distanceJoint.maxDistanceOnly = true;
        distanceJoint.autoConfigureDistance = false;
        distanceJoint.autoConfigureConnectedAnchor = false;
        distanceJoint.enabled = false;

        ropeCollider = GetComponentInChildren<RopeCollider>();
        lineRenderer = GetComponentInChildren<LineRenderer>();

        Disable();
    }


    /// <summary>
    /// Método que gestiona la colisión del objeto que representa la cabeza del gancho
    /// </summary>
    public void Collide(Collision2D collision)
    {
        if (isBeingThrown)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("HookableLayer"))
            {
                headRigidbody.velocity = Vector2.zero;
                Attach(headRigidbody.position);
                isBeingThrown = false;
            }
        }
    }
}

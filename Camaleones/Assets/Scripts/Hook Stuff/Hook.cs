using UnityEngine;

#pragma warning disable 649
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

    [Header("Fuerza al enganchar al terreno")]
    [Tooltip("La magnitud de la fuerza que se le aplica al cuerpo conectado cuando el gancho se engancha a algo.")]
    public float forceOnAttached = 10;
    [Tooltip("La fuerza al enganchar sólo se aplica si el cuerpo conectado va a menos que esta velocidad.")]
    public float maxVelocityForForceOnAttached = 20;

    [Header("Fuerza al enganchar a otro jugador")]
    public float hookerVelocityOnPlayerHooked = 5;
    public float hookedVelocityOnPlayerHooked = 1;

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
            if (ConnectedBody) Physics2DExtensions.IgnoreCollisions(ConnectedBody, headRigidbody, false);

            distanceJoint.connectedBody = value;
            connectedBodyTransform = value.GetComponent<Transform>();
        }
    }
    public Vector2 HeadPosition { get => headRigidbody.position; }
    #endregion

    #region References
    private DistanceJoint2D distanceJoint;
    private FixedJoint2D fixedJoint;
    private Collider2D headCollider;
    private RopeCollider ropeCollider;
    private LineRenderer lineRenderer;
    private Transform connectedBodyTransform;
    #endregion

    private bool isBeingThrown;
    private Vector2 throwOriginPoint;

    #region Acciones principales
    /// <summary>
    /// Metodo que se llama cuando se lanza el gancho.
    /// Recibe el punto hacia el cual se lanza el gancho y el RigidBody del personaje que lo lanza
    /// </summary>
    public virtual void Throw(Vector2 targetPoint)
    {
        gameObject.SetActive(true);
        IsAttached = false;
        isBeingThrown = true;

        throwOriginPoint = ConnectedBody.position;
        Vector2 throwDirection = (targetPoint - throwOriginPoint).normalized;

        headRigidbody.isKinematic = false;
        headRigidbody.position = throwOriginPoint;
        headRigidbody.velocity = throwDirection * hookProjectileSpeed;
        headRigidbody.transform.position = throwOriginPoint;

        headCollider.enabled = true;

        distanceJoint.enabled = false;
        fixedJoint.enabled = false;

        //El estado de IgnoreCollisions se deshace cuando los colliders se desactivan, así que hacemos esto cada vez que lanzamos el gancho.
        Physics2DExtensions.IgnoreCollisions(ConnectedBody, headRigidbody, true);
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
    }

    /// <summary>
    /// Método que se llama cuando la cabeza del gancho entra en contacto con una plataforma tras ser lanzada.
    /// </summary>
    protected virtual void AttachToPoint(Vector2 attachPoint)
    {
        //Aplicar velocidad al lanzador
        if (ConnectedBody.velocity.magnitude < maxVelocityForForceOnAttached)
        {
            Vector2 attachForceOnThrower = (attachPoint - ConnectedBody.position).normalized * forceOnAttached;
            ConnectedBody.AddForce(attachForceOnThrower, ForceMode2D.Impulse);
        }

        headRigidbody.position = attachPoint;
        headRigidbody.isKinematic = true;
        headRigidbody.velocity = Vector2.zero;

        headCollider.enabled = false;

        ropeCollider.HeadPosition = attachPoint;
        distanceJoint.distance = ropeCollider.SwingingSegmentLength;
        distanceJoint.enabled = true;

        IsAttached = true;
        isBeingThrown = false;
    }

    protected virtual void AttachToRigidbody(Rigidbody2D rigidbodyToAttachTo)
    {
        Vector2 u = (rigidbodyToAttachTo.position - ConnectedBody.position).normalized;

        //Dar velocidades a los cuerpos conectados
        ConnectedBody.velocity = hookerVelocityOnPlayerHooked * u;
        rigidbodyToAttachTo.velocity = hookedVelocityOnPlayerHooked * -u;

        //Configurar el gancho
        headRigidbody.isKinematic = false;
        headRigidbody.position = rigidbodyToAttachTo.position;
        headRigidbody.velocity = Vector2.zero;

        headCollider.enabled = false;

        distanceJoint.distance = ropeCollider.SwingingSegmentLength;
        distanceJoint.enabled = true;

        fixedJoint.connectedBody = rigidbodyToAttachTo;
        fixedJoint.enabled = true;

        //Cambiar el estado
        IsAttached = true;
        isBeingThrown = false;

        //Si hemos enganchado a otro jugador, desactivar su lengua
        rigidbodyToAttachTo.GetComponent<HookThrower>()?.LetGo();
    }
    #endregion

    /// <summary>
    /// Método que gestiona la colisión del objeto que representa la cabeza del gancho
    /// </summary>
    public void Collide(Collision2D collision)
    {
        if (isBeingThrown)
        {
            if (collision.collider.gameObject.layer == LayerMask.NameToLayer("HookableEntityLayer"))
            {
                AttachToRigidbody(collision.collider.attachedRigidbody);
            }
            else if (collision.gameObject.layer == LayerMask.NameToLayer("HookableTerrainLayer"))
            {
                AttachToPoint(headRigidbody.position);
            }
        }
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

    #region Inicializión
    protected virtual void Awake()
    {
        enabled = true; //Al no tener Start ni Update, enabled==false por defecto. Lo ponemos a true para que HookThrower sepa si el gancho está activo.

        headCollider = headRigidbody.GetComponent<Collider2D>();

        distanceJoint = GetComponentInChildren<DistanceJoint2D>();
        distanceJoint.enableCollision = false;
        distanceJoint.maxDistanceOnly = true;
        distanceJoint.autoConfigureDistance = false;
        distanceJoint.autoConfigureConnectedAnchor = false;

        fixedJoint = GetComponentInChildren<FixedJoint2D>();
        fixedJoint.enableCollision = false;
        fixedJoint.autoConfigureConnectedAnchor = true;
        fixedJoint.connectedAnchor = Vector2.zero;

        ropeCollider = GetComponentInChildren<RopeCollider>();
    }

    private void Start()
    {
        Disable();
    }
    #endregion
}

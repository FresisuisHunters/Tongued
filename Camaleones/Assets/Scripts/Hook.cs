using UnityEngine;

/// <summary>
/// Componente central del gancho. Maneja activarlo, engancharlo y desactivarlo; 
/// y da la información necesaria a los otros componentes del gancho.
/// El gancho está dividido en dos partes: la cabeza (la parte que se engancha) 
/// y el swinging point (el punto del que cuelga el jugador, que puede moverse en algunos casos.
/// 
/// 
/// </summary>
[DisallowMultipleComponent]
public class Hook : MonoBehaviour
{
    #region Inspector
    [Tooltip ("La magnitud de la fuerza que se le aplica al cuerpo conectado cuando el gancho se engancha a algo.")]
    public float forceOnAttached = 10;
    [Tooltip ("La fuerza al enganchar sólo se aplica si el cuerpo conectado va a menos que esta velocidad.")]
    public float maxVelocityForForceOnAttached = 20;

    [SerializeField] protected Rigidbody2D headRigidbody;
    #endregion

    #region Propiedades Públicas
    public bool IsOut => isActiveAndEnabled;
    public bool IsAttached { get; private set; }
    /// <summary>
    /// La longitud de cuerda de la que cuelga el cuerpo conectado
    /// </summary>
    public float Length { get => distanceJoint.distance; set => distanceJoint.distance = value; }
    public Rigidbody2D ConnectedBody { get => distanceJoint.connectedBody; set => distanceJoint.connectedBody = value; }
    #endregion

    protected DistanceJoint2D distanceJoint;
    protected RopeCollider ropeCollider;
    protected LineRenderer lineRenderer;

    public virtual void Throw (Vector2 targetPoint)
    {
        gameObject.SetActive (true);
        IsAttached = false;

        headRigidbody.position = targetPoint;

        Attach(targetPoint);
    }

    public virtual void Disable ()
    {
        gameObject.SetActive (false);
        IsAttached = false;

        ropeCollider.ClearContacts();
    }


    //Ahora mismo se llama a la que se echa el gancho, pero más tarde el gancho será un proyectil y Attach() se llamará una vez choque con algo.
    protected virtual void Attach (Vector2 attachPoint)
    {
        headRigidbody.position = attachPoint;

        distanceJoint.distance = Vector2.Distance(attachPoint, ConnectedBody.position);
        distanceJoint.enabled = true;

        //Fuerza de enganche. Sólo se aplica si el cuerpo conectado va a menos que cierta velocidad.
        if (ConnectedBody.velocity.magnitude < maxVelocityForForceOnAttached) {
            Vector2 attachForceOnThrower = (attachPoint - ConnectedBody.position).normalized * forceOnAttached;
            ConnectedBody.AddForce(attachForceOnThrower);
        }

        IsAttached = true;
    }

    private void FixedUpdate () {
        //Actualiza las posiciones finales de RopeCollider.
        ropeCollider.freeSwingingEndPoint = distanceJoint.connectedBody.position;
        ropeCollider.HeadPosition = headRigidbody.position;
    }


    //Visualización provisional
    private void Update ()
    {
        Vector3[] ropePoints = ropeCollider.GetRopePoints();

        lineRenderer.positionCount = ropePoints.Length;
        lineRenderer.SetPositions(ropePoints);
    }

    protected virtual void Awake ()
    {
        enabled = true; //Al no tener Start ni Update, enabled==false por defecto. Lo ponemos a true para que HookThrower sepa si el gancho está activo.

        distanceJoint = GetComponentInChildren<DistanceJoint2D> ();
        distanceJoint.enableCollision = false;
        distanceJoint.maxDistanceOnly = true;
        distanceJoint.autoConfigureDistance = false;
        distanceJoint.autoConfigureConnectedAnchor = false;

        ropeCollider = GetComponentInChildren<RopeCollider> ();
        lineRenderer = GetComponentInChildren<LineRenderer>();

        Disable();
    }
}
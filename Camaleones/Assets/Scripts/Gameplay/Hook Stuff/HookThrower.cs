using System;
using UnityEngine;
using UnityEngine.EventSystems;

#pragma warning disable 649
/// <summary>
/// Componente encargado de dar órdenes al gancho.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class HookThrower : MonoBehaviour, IOnHookedListener
{
    #region Inspector
    [Header("Hook")]
    public float retractDistancePerSecond = 10f;
    [SerializeField] private Hook hookPrefab;

    [Header("Autoaim")]
    [SerializeField] private int autoAimRayCount = 5;
    [SerializeField] private float autoAimConeAngle = 10;
    [SerializeField] private LayerMask autoAimLayerMask;

    [Header("Debug")]
    public bool debugAutoAim;
    public bool debugVelocity;
    #endregion

    #region Eventos
    //Duplica los eventos de Hook. Se hace para que componentes como ChamaleonAnimator puedan suscribirse a los eventos independientemente de si los Hooks remotos se han creado ya.
    //Componentes en el camaleón deberían suscribirse a ESTOS, no a los de Hook.
    public event Action<Vector2> OnHookThrown;
    public event Action OnHookAttached;
    public event Action OnHookDisabled;
    #endregion

    #region Propiedades Públicas
    public bool HookIsOut => Hook?.IsOut ?? false;
    public Hook Hook
    {
        get => _hook;
        set
        {
            _hook = value;
            if (value != null)
            {
                value.OnAttached += () => OnHookAttached?.Invoke();
                value.OnThrown += (Vector2 targetPoint) => OnHookThrown?.Invoke(targetPoint);
                value.OnDisabled += () => OnHookDisabled?.Invoke();
            }
        }
    }
    private Hook _hook;
    public Rigidbody2D Rigidbody { get; private set; }
    public Vector2 SwingingHingePoint => Hook.SwingingHingePoint;
    #endregion

    private RaycastHit2D[] raycastHits = new RaycastHit2D[1];


    public void ThrowHook(Vector2 requestedPoint)
    {
        bool originalQueriesStartInColliders = Physics2D.queriesStartInColliders;
        Physics2D.queriesStartInColliders = false;

        //Lanzamos varios rayos en un ángulo de apertura para hacer el autoaim.
        //De los hits que hemos tenido, nos quedamos con el más cercano a requestedPoint.
        //Si no hay hits, utilizamos requestedPoint.
        //TODO: Utilizar requestedPoint si se encuentra un jugador en los hits.

        Vector2 rbPosition = Rigidbody.position;
        Vector2 u = (requestedPoint - rbPosition).normalized;
        Vector2 origin;
        Vector2 direction;

        float angle = -autoAimConeAngle / 2 * Mathf.Deg2Rad;
        float deltaAngle = (autoAimConeAngle * Mathf.Deg2Rad) / autoAimRayCount;
        int hitCount;

        Vector2 finalPoint = requestedPoint;
        bool hitPlayer = false;

        float squaredDistanceFromRequestedToFinalPoint = float.MaxValue;
        float squaredDistance;
        RaycastHit2D hit;
        int hitLayer;

        for (int i = 0; i < autoAimRayCount && !hitPlayer; i++)
        {
            //Rota el vector u por angle
            direction.x = u.x * Mathf.Cos(angle) - u.y * Mathf.Sin(angle);
            direction.y = u.x * Mathf.Sin(angle) + u.y * Mathf.Cos(angle);

            origin = rbPosition + direction * Hook.minEntityHookDistance;

            hitCount = Physics2D.RaycastNonAlloc(origin, direction, raycastHits, Hook.maxHookDistance, autoAimLayerMask);

            for (int j = 0; j < hitCount && !hitPlayer; j++)
            {
                hit = raycastHits[j];
                hitLayer = hit.collider.gameObject.layer;

                //Si tocamos a un jugador (y no somos nosotros), no hay autoaim.
                if (hitLayer == LayerMask.NameToLayer("HookableEntityLayer") && hit.rigidbody != Rigidbody && hit.distance > Hook.minEntityHookDistance)
                {
                    finalPoint = requestedPoint;
                    hitPlayer = true;
                }
                else if (hitLayer == LayerMask.NameToLayer("HookableTerrainLayer"))
                {
                    squaredDistance = (origin - raycastHits[j].point).sqrMagnitude;
                    if (squaredDistance < squaredDistanceFromRequestedToFinalPoint)
                    {
                        squaredDistanceFromRequestedToFinalPoint = squaredDistance;
                        finalPoint = raycastHits[j].point;
                    }
                }
            }
            angle += deltaAngle;

            if (debugAutoAim) Debug.DrawRay(origin, direction * 1000, Color.white, 2);
        }

        Hook.Throw(finalPoint);

        if (debugAutoAim)
        {
            Debug.DrawLine(rbPosition, finalPoint, Color.red, 2);
        }

        Physics2D.queriesStartInColliders = originalQueriesStartInColliders;
    }

    public void Retract(float time, float amountMultiplier)
    {
        if (Hook.IsAttached)
        {
            Hook.Length -= retractDistancePerSecond * amountMultiplier * time;

            Vector2 u = Hook.SwingingHingePoint - Rigidbody.position;
            Rigidbody.AddForce(u * retractDistancePerSecond * time, ForceMode2D.Impulse);
        }
    }

    public void DisableHook()
    {
        Hook.Disable();
    }

    void IOnHookedListener.OnHooked(Vector2 pullDirection, Hook hook) => DisableHook();

    #region Initialization
    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        //Si estamos jugando online (tenemos un PhotonView) y somos el jugador local, utilizamos PhotonNetwork para instanciar el gancho. Si no, un Instantiate de toda la vida.
        //En el prefab  online se asigna OnlineHook, en el prefab offline se asigna Hook.
        Photon.Pun.PhotonView photonView = GetComponent<Photon.Pun.PhotonView>();
        if (!photonView) Hook = Instantiate(hookPrefab);
        else if (photonView.IsMine) Hook = Photon.Pun.PhotonNetwork.Instantiate(hookPrefab.name, Vector3.zero, Quaternion.identity, data: new object[] { photonView.ViewID }).GetComponent<Hook>();

        if (Hook) Hook.ConnectedBody = Rigidbody;
    }
    #endregion

    //Debug only
    private void OnGUI()
    {
        if (debugVelocity) GUI.TextArea(new Rect(0, 0, 170, 30), $"{name}'s velocity: {Rigidbody.velocity.magnitude.ToString()}");
    }
}

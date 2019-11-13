using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

#pragma warning disable 649
/// <summary>
/// Componente encargado de dar órdenes al gancho.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class HookThrower : MonoBehaviour
{
    public float retractDistancePerSecond = 10f;
    [SerializeField] private Hook hookPrefab;

    [SerializeField] private int autoAimRayCount = 5;
    [SerializeField] private float autoAimConeAngle = 10;

    [SerializeField] private LayerMask autoAimLayerMask;

    public bool HookIsOut => Hook.IsOut;

    public Hook Hook { get; set; }
    public Rigidbody2D Rigidbody { get; private set; }

    private RaycastHit2D[] raycastHits = new RaycastHit2D[1];

    public void ThrowHook(Vector2 requestedPoint)
    {
        //Lanzamos varios rayos en un ángulo de apertura para hacer el autoaim.
        //De los hits que hemos tenido, nos quedamos con el más cercano a requestedPoint.
        //Si no hay hits, utilizamos requestedPoint.
        //TODO: Utilizar requestedPoint si se encuentra un jugador en los hits.

        Vector2 origin = Rigidbody.position;
        Vector2 u = (requestedPoint - origin).normalized;
        Vector2 direction;

        float angle = -autoAimConeAngle * Mathf.Deg2Rad;
        float deltaAngle = (autoAimConeAngle * Mathf.Deg2Rad) / autoAimRayCount;
        int hitCount;

        Vector2 finalPoint = Vector2.zero;
        float squaredDistanceFromRequestedToFinalPoint = float.MaxValue;
        float squaredDistance;

        for (int i = 0; i < autoAimRayCount; i++)
        {
            //Rota el vector u por angle
            direction.x = u.x * Mathf.Cos(angle) - u.y * Mathf.Sin(angle);
            direction.y = u.x * Mathf.Sin(angle) + u.y * Mathf.Cos(angle);

            hitCount = Physics2D.RaycastNonAlloc(origin, direction, raycastHits, float.MaxValue, autoAimLayerMask);

            for (int j = 0; j < hitCount; j++)
            {
                squaredDistance = (origin - raycastHits[j].point).sqrMagnitude;
                if (squaredDistance < squaredDistanceFromRequestedToFinalPoint)
                {
                    squaredDistanceFromRequestedToFinalPoint = squaredDistance;
                    finalPoint = raycastHits[j].point;
                }
            }
            angle += deltaAngle;
        }

        if (squaredDistanceFromRequestedToFinalPoint == float.MaxValue) finalPoint = requestedPoint;

        Hook.Throw(finalPoint);
    }

    public void Retract(float time)
    {
        if (Hook.IsAttached) Hook.Length -= retractDistancePerSecond * time;
    }

    public void LetGo()
    {
        Hook.Disable();
    }

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody2D>();

        //Si estamos jugando online (tenemos un PhotonView) y somos el jugador local, utilizamos PhotonNetwork para instanciar el gancho. Si no, un Instantiate de toda la vida.
        //En el prefab  online se asigna OnlineHook, en el prefab offline se asigna Hook.
        Photon.Pun.PhotonView photonView = GetComponent<Photon.Pun.PhotonView>();
        if (!photonView) Hook = Instantiate(hookPrefab);
        else if (photonView.IsMine) Hook = Photon.Pun.PhotonNetwork.Instantiate(hookPrefab.name, Vector3.zero, Quaternion.identity, data: new object[]{ photonView.ViewID}).GetComponent<Hook>();

        if (Hook) Hook.ConnectedBody = Rigidbody;
    }
}

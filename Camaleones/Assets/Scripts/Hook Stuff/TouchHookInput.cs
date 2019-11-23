using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;

[RequireComponent (typeof (HookThrower))]
public class TouchHookInput : MonoBehaviour {

    private const float INNER_CIRCLE_RADIUS = 10f;
    private const float OUTER_CIRCLE_RADIUS = 20f;

    private HookThrower hookThrower;
    private Vector2 positionTouchedScreenCoordinates;
    private Vector2 positionTouchedWorldCoordinates;
    private bool touchingScreen;
    private bool retract;

    private void Awake() {
        hookThrower = GetComponent<HookThrower>();
        touchingScreen = false;
        retract = false;
    }

    private void Start() {
        PhotonView photonView = GetComponent<PhotonView>();
        if (photonView && !photonView.IsMine) {
            Destroy(this);
            return;
        }

        //Si el control seleccionado no es Touch, se autodestruye.
        if ((ControlScheme) PlayerPrefs.GetInt(SettingsMenuScreen.CONTROL_SCHEME_PREF_KEY, 0) != ControlScheme.Touch)
        {
            Destroy(this);
            return;
        }

        InputEventReceiver inputEventReceiver = FindObjectOfType<InputEventReceiver>();
        if (!inputEventReceiver) {
            OnlineLogging.Instance.Write("TouchHookInput necesita de InputEventReceiver");
            return;
        }

        inputEventReceiver.AddListener(EventTriggerType.PointerDown, (data) => { if (enabled) OnPointerDown(data as PointerEventData); });
        inputEventReceiver.AddListener(EventTriggerType.PointerUp, (data) => { if (enabled) OnPointerUp(); });
        inputEventReceiver.AddListener(EventTriggerType.Drag, (data) => { if (enabled) OnPointerDrag(data as PointerEventData); });
    }

    private void Update() {
        if (retract) {
            hookThrower.Retract(Time.deltaTime);
        }
    }

    private void OnPointerDown(PointerEventData eventData) {
        if (touchingScreen) {
            return;
        }

        touchingScreen = true;
        positionTouchedScreenCoordinates = eventData.position;
        if (!hookThrower.HookIsOut) {
            positionTouchedWorldCoordinates = eventData.pressEventCamera.ScreenToWorldPoint(positionTouchedScreenCoordinates);
            hookThrower.ThrowHook(positionTouchedWorldCoordinates);
        }
    }

    private void OnPointerUp() {
        touchingScreen = false;
        retract = false;
        hookThrower.DisableHook();
    }

    private void OnPointerDrag(PointerEventData eventData) {
        Vector2 screenPosition = eventData.position;
        float distanceToOriginalTouch = Vector2.Distance(positionTouchedScreenCoordinates, screenPosition);

        Debug.Log(distanceToOriginalTouch);

        retract = distanceToOriginalTouch >= INNER_CIRCLE_RADIUS;
    }

    void OnDrawGizmos() {
        if (!touchingScreen) {
            return;
        }

        Vector3 gizmoPosition = new Vector3(positionTouchedWorldCoordinates.x, positionTouchedWorldCoordinates.y, -5);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(gizmoPosition, OUTER_CIRCLE_RADIUS);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(gizmoPosition, INNER_CIRCLE_RADIUS);
    }

}
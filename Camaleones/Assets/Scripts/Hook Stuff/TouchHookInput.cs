using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;

[RequireComponent (typeof (HookThrower))]
public class TouchHookInput : MonoBehaviour {

    private const float DISTANCE_ACTIVATE_RECTRACT_SCREEN_SPACE = 30f;

    private HookThrower hookThrower;
    private Vector2 positionTouchedScreenCoordinates;
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

        InputEventReceiver inputEventReceiver = FindObjectOfType<InputEventReceiver>();
        if (!inputEventReceiver) {
            OnlineLogging.Instance.Write("TouchHookInput necesita de InputEventReceiver");
            return;
        }

        inputEventReceiver.AddListener(EventTriggerType.PointerDown, (data) => OnPointerDown(data as PointerEventData));
        inputEventReceiver.AddListener(EventTriggerType.PointerUp, (data) => OnPointerUp());
        inputEventReceiver.AddListener(EventTriggerType.Drag, (data) => OnPointerDrag(data as PointerEventData));
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
            Vector2 positionTouchedWorldCoordinates = eventData.pressEventCamera.ScreenToWorldPoint(positionTouchedScreenCoordinates);
            hookThrower.ThrowHook(positionTouchedWorldCoordinates);
        }
    }

    private void OnPointerUp() {
        touchingScreen = false;
        retract = false;
        hookThrower.LetGo();
    }

    private void OnPointerDrag(PointerEventData eventData) {
        Vector2 screenPosition = eventData.position;
        float distanceToOriginalTouch = Vector2.Distance(positionTouchedScreenCoordinates, screenPosition);

        Debug.Log(distanceToOriginalTouch);

        retract = distanceToOriginalTouch >= DISTANCE_ACTIVATE_RECTRACT_SCREEN_SPACE;
    }

}
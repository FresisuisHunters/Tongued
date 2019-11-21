using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;

[RequireComponent (typeof (HookThrower))]
public class TouchHookInput : MonoBehaviour {

    private HookThrower hookThrower;
    private bool touchingScreen;
    private Vector3 positionTouchedScreenCoordinates;

    private void Awake() {
        hookThrower = GetComponent<HookThrower>();
        touchingScreen = false;
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
    }

    private void Update() {
        if (hookThrower.HookIsOut && touchingScreen) {
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
            Vector3 positionTouchedWorldCoordinates = eventData.pressEventCamera.ScreenToWorldPoint(positionTouchedScreenCoordinates);
            hookThrower.ThrowHook(positionTouchedWorldCoordinates);
        }
    }

    private void OnPointerUp() {
        touchingScreen = false;
        hookThrower.LetGo();
    }

}
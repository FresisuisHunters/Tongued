using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;
using UnityEngine.UI;

[RequireComponent (typeof (HookThrower))]
public class TouchHookInput : MonoBehaviour {

    private const float INNER_CIRCLE_RADIUS = 10f;
    private const float OUTER_CIRCLE_RADIUS = 20f;

    private HookThrower hookThrower;
    private Vector2 positionTouchedScreenCoordinates;
    private Vector2 positionTouchedWorldCoordinates;
    public Image innerCircleSprite;
    public Image outerCircleSprite;
    private bool touchingScreen;
    private bool retract;

    private void Awake() {
        hookThrower = GetComponent<HookThrower>();
        touchingScreen = false;
        retract = false;

        innerCircleSprite = GameObject.FindGameObjectWithTag("InnerCircle").GetComponent<Image>();
        outerCircleSprite = GameObject.FindGameObjectWithTag("OuterCircle").GetComponent<Image>();
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
            positionTouchedWorldCoordinates = eventData.pressEventCamera.ScreenToWorldPoint(positionTouchedScreenCoordinates);
            hookThrower.ThrowHook(positionTouchedWorldCoordinates);

            innerCircleSprite.gameObject.SetActive(true);
            outerCircleSprite.gameObject.SetActive(true);
            innerCircleSprite.rectTransform.localPosition = positionTouchedScreenCoordinates;
            outerCircleSprite.rectTransform.localPosition = positionTouchedScreenCoordinates;
        }
    }

    private void OnPointerUp() {
        touchingScreen = false;
        retract = false;
        hookThrower.LetGo();

        innerCircleSprite.gameObject.SetActive(false);
        outerCircleSprite.gameObject.SetActive(false);
    }

    private void OnPointerDrag(PointerEventData eventData) {
        Vector2 screenPosition = eventData.position;
        float distanceToOriginalTouch = Vector2.Distance(positionTouchedScreenCoordinates, screenPosition);

        outerCircleSprite.rectTransform.localPosition = screenPosition;

        retract = distanceToOriginalTouch >= INNER_CIRCLE_RADIUS;
    }

}
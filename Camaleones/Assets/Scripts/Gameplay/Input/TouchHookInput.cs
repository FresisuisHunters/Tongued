using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;
using UnityEngine.UI;

[RequireComponent (typeof (HookThrower))]
public class TouchHookInput : MonoBehaviour {

    #region Inspector
    [Header("Retract parameters")]
    public float retractDeadzoneRadius = 10f;
    public float retractMaxRadius = 20f;

    [Header("Current retract visualization")]
    public Gradient currentRetractColorRamp;
    public float maxRotationSpeed = 180;

    [Header("Prefabs")]
    public RectTransform retractCurrentPrefab;
    public RectTransform retractMaxPrefab;
    #endregion

    #region References
    private RectTransform canvasTransform;
    private RectTransform retractCurrentTransform;
    private Image retractCurrentImage;
    private RectTransform retractMaxTransform;
    private HookThrower hookThrower;
    #endregion

    #region Private State
    private Vector2 positionTouchedScreenCoordinates;
    private Vector2 positionTouchedWorldCoordinates;

    private bool isTouchingScreen;
    private float currentRetractAmount;
    #endregion
    
    private void Update()
    {
        if (isTouchingScreen && currentRetractAmount > 0)
        {
            hookThrower.Retract(Time.deltaTime, currentRetractAmount);
            UpdateCircleAnimation();
        }
    }

    #region Events
    private void OnPointerDown(PointerEventData eventData)
    {
        if (isTouchingScreen) return;

        isTouchingScreen = true;
        positionTouchedScreenCoordinates = eventData.position;

        if (!hookThrower.HookIsOut)
        {
            //Lanza el gancho
            positionTouchedWorldCoordinates = eventData.pressEventCamera.ScreenToWorldPoint(positionTouchedScreenCoordinates);
            hookThrower.ThrowHook(positionTouchedWorldCoordinates);

            //Prepara y activa los círculos
            SetCirclePosition(eventData.position, eventData.pressEventCamera);
            SetCircleSizes(0, retractMaxRadius);
            SetCirclesActive(true);
            currentRetractAmount = 0;
        }
    }

    private void OnPointerUp()
    {
        isTouchingScreen = false;
        hookThrower.DisableHook();

        retractCurrentTransform.gameObject.SetActive(false);
        retractMaxTransform.gameObject.SetActive(false);
    }

    private void OnPointerDrag(PointerEventData eventData)
    {
        Vector2 screenPosition = eventData.position;
        float distanceToOriginalTouch = Vector2.Distance(positionTouchedScreenCoordinates, screenPosition);

        currentRetractAmount = Mathf.Clamp01(Mathf.InverseLerp(retractDeadzoneRadius, retractMaxRadius, distanceToOriginalTouch));
        SetCircleSizes(distanceToOriginalTouch, retractMaxRadius);
        UpdateCurrentRetractColor();
    }
    #endregion

    #region Circle Actions
    private void SetCirclesActive(bool active)
    {
        retractCurrentTransform.gameObject.SetActive(active);
        retractMaxTransform.gameObject.SetActive(active);
    }

    private void SetCirclePosition(Vector2 screenSpacePosition, Camera camera)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasTransform, positionTouchedScreenCoordinates, camera, out Vector2 localPos);

        retractCurrentTransform.localPosition = localPos;
        retractMaxTransform.localPosition = localPos;
    }

    private void SetCircleSizes(float currentRadius, float maxRadius)
    {
        float clampedCurrentRadius = Mathf.Clamp(currentRadius, 0, maxRadius);
        retractCurrentTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, clampedCurrentRadius * 2);
        retractCurrentTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, clampedCurrentRadius * 2);

        retractMaxTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxRadius * 2);
        retractMaxTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, maxRadius * 2);
    }

    private void UpdateCurrentRetractColor()
    {
        retractCurrentImage.color = currentRetractColorRamp.Evaluate(currentRetractAmount);
    }

    private void UpdateCircleAnimation()
    {
        retractMaxTransform.Rotate(0, 0, maxRotationSpeed * currentRetractAmount * Time.deltaTime);
    }
    #endregion

    #region Initialization
    private void Awake()
    {
        //Si el control seleccionado no es Touch, se autodestruye.
        if ((ControlScheme)PlayerPrefs.GetInt(SettingsMenuScreen.CONTROL_SCHEME_PREF_KEY, 0) != ControlScheme.Touch)
        {
            Destroy(this);
            return;
        }

        hookThrower = GetComponent<HookThrower>();
        isTouchingScreen = false;
    }

    private void Start()
    {
        InputEventReceiver inputEventReceiver = FindObjectOfType<InputEventReceiver>();
        if (!inputEventReceiver)
        {
            OnlineLogging.Instance.Write("TouchHookInput necesita de InputEventReceiver");
            return;
        }

        inputEventReceiver.AddListener(EventTriggerType.PointerDown, (data) => { if (this && enabled) OnPointerDown(data as PointerEventData); });
        inputEventReceiver.AddListener(EventTriggerType.PointerUp, (data) => { if (this && enabled) OnPointerUp(); });
        inputEventReceiver.AddListener(EventTriggerType.Drag, (data) => { if (this && enabled) OnPointerDrag(data as PointerEventData); });

        canvasTransform = inputEventReceiver.GetComponent<RectTransform>();
        retractCurrentTransform = Instantiate(retractCurrentPrefab, canvasTransform, false);
        retractMaxTransform = Instantiate(retractMaxPrefab, canvasTransform, false);
        retractCurrentImage = retractCurrentTransform.GetComponent<Image>();
        SetCirclesActive(false);
    }
    #endregion
}
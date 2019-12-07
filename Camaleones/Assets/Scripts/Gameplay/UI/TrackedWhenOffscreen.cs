using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 649
public class TrackedWhenOffscreen : MonoBehaviour
{
    #region Inspector
    [Header("View")]
    [SerializeField] private RectTransform offscreenViewPrefab;
    [SerializeField] private Vector2 baseViewFacing;

    [Header("Placement")]
    [SerializeField] private float marginSize = 40f;
    [SerializeField] private Vector2 canvasReferenceResolution = new Vector2(1920, 1080);
    #endregion

    #region References
    public RectTransform ViewTransform { get; private set; }

    private Transform trackedObject;
    private new Camera camera;
    #endregion

    #region Properties
    public bool IsActive { get => ViewTransform?.gameObject.activeSelf ?? false; set => ViewTransform?.gameObject.SetActive(value); }

    private bool ObjectIsInView
    {
        get
        {
            Vector3 positionInMainViewport = camera.WorldToViewportPoint(trackedObject.position);

            return (positionInMainViewport.x > 0 && positionInMainViewport.x < 1 &&
                positionInMainViewport.y > 0 && positionInMainViewport.y < 1);
        }
    }
    #endregion

    #region Update
    private void LateUpdate()
    {
        if (!camera) return;

        if (ObjectIsInView)
        {
            if (IsActive) IsActive = false;
        }
        else
        {
            if (!IsActive) IsActive = true;
        }

        if (IsActive)
        {
            PositionView();
        }
    }

    private void PositionView()
    {
        Vector3 newPos = transform.position;

        Vector3 unclampedPos = camera.WorldToViewportPoint(trackedObject.position);

        Vector3 normalizedPos;
        normalizedPos.x = Mathf.Clamp01(unclampedPos.x);
        normalizedPos.y = Mathf.Clamp01(unclampedPos.y);
        normalizedPos.z = unclampedPos.z;

        ViewTransform.anchorMin = normalizedPos;
        ViewTransform.anchorMax = normalizedPos;

        Rect viewRect = ViewTransform.rect;
        Vector2 anchoredPosition = Vector2.zero;

        if (normalizedPos.x == 0) anchoredPosition.x = viewRect.width / 2;
        else if (normalizedPos.x == 1) anchoredPosition.x = -viewRect.width / 2;

        if (normalizedPos.y == 0) anchoredPosition.y = viewRect.height / 2;
        else if (normalizedPos.y == 1) anchoredPosition.y = -viewRect.height / 2;

        ViewTransform.anchoredPosition = anchoredPosition;

        RotateView(unclampedPos);
    }

    private void RotateView(Vector2 viewportSpacePositionOfTarget)
    {
        float angle = Vector2.SignedAngle(baseViewFacing, viewportSpacePositionOfTarget - new Vector2(0.5f, 0.5f));
        ViewTransform.rotation = Quaternion.Euler(0, 0, angle);
    }
    #endregion

    private void OnDisable()
    {
        IsActive = false;
    }

    #region Initialization
    private void Awake()
    {
        //Get the references
        trackedObject = GetComponent<Transform>();
        camera = Camera.main;

        CreateView();
    }

    private void CreateView()
    {
        //Create the canvas
        GameObject canvasGO = new GameObject(name + "'s Offscreen Tracking Canvas");
        canvasGO.layer = LayerMask.NameToLayer("UI");

        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler canvasScaler = canvasGO.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        canvasScaler.matchWidthOrHeight = 0.5f;
        canvasScaler.referenceResolution = canvasReferenceResolution;

        //Add the parent that holds the margin
        RectTransform marginHolder = new GameObject("Margin", typeof(RectTransform)).GetComponent<RectTransform>();
        marginHolder.SetParent(canvas.GetComponent<RectTransform>());

        marginHolder.anchorMin = Vector2.zero;
        marginHolder.anchorMax = Vector2.one;
        marginHolder.offsetMin = new Vector2(marginSize, marginSize);
        marginHolder.offsetMax = new Vector2(-marginSize, -marginSize);

        //Add the view
        ViewTransform = Instantiate(offscreenViewPrefab, marginHolder);
        IsActive = false;
    }

    private void OnDestroy()
    {
        if (ViewTransform) Destroy(ViewTransform.parent.parent.gameObject);
    }
    #endregion
}
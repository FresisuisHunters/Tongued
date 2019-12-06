using UnityEngine;
using UnityEngine.UI;
using TMPro;

#pragma warning disable 649
[RequireComponent(typeof(HotPotatoHandler), typeof(Animator), typeof(PlayersHandler))]
public class HotPotatoUI : MonoBehaviour
{
    #region Inspector
    [Header("Timer")]
    [SerializeField] private Slider timeLeftInRoundSlider;
    [SerializeField] private Image sliderFillImage;
    [SerializeField] private Sprite blessingSliderFill;
    [SerializeField] private Sprite curseSliderFill;
    [SerializeField] private Image sliderBackgroundImage;
    [SerializeField] private Color blessingSliderBackgroundColor;
    [SerializeField] private Color curseSliderBackgroundColor;

    [Header("Start countdown")]
    [SerializeField] private TextMeshProUGUI countdownTextField;
    [SerializeField] private int shownCountdownLength = 5;
    [SerializeField] private GameObject roundCounterLabel;

    [Header("Round tint")]
    [SerializeField] private Color blessingRoundTint = Color.green;
    [SerializeField] private Color curseRoundTint = Color.red;
    [SerializeField] private Graphic[] graphicsToTintOnRoundChange;

    [SerializeField] private TextMeshProUGUI[] jungleFeverTextsToTintOnRoundChange;
    [SerializeField] private Material jungleFeverBlessingTextMaterial;
    [SerializeField] private Material jungleFeverCurseTextMaterial;

    [SerializeField] private TextMeshProUGUI[] janBradyTextsToTintOnRoundChange;
    [SerializeField] private Material janBradyBlessingTextMaterial;
    [SerializeField] private Material janBradyCurseTextMaterial;

    [SerializeField] private Image totemImage;

    [Header("Round counter")]
    [SerializeField] private TextMeshProUGUI roundCounterTextField;

    [Header("Mission Text")]
    [SerializeField] private TextMeshProUGUI missionText;
    [SerializeField] private string msgBlessingHasTotem = "Keep the totem!";
    [SerializeField] private string msgBlessingDoesntHaveTotem = "Get the totem!";
    [SerializeField] private string msgCurseHasTotem = "Get rid of the totem!";
    [SerializeField] private string msgCurseDoesntHaveTotem = "Keep off the totem!";

    [Header("Snitch sprites")]
    [SerializeField] private Sprite blessingRoundSnitchSprite;
    [SerializeField] private Sprite curseRoundSnitchSprite;

    [Header("Totem sprites")]
    [SerializeField] private Sprite blessingTotemSprite;
    [SerializeField] private Sprite curseTotemSprite;

    [Header("Round change animation")]
    [SerializeField] private Image roundChangeAnimationTotemImage;

    [Header("Target Detector")]
    [SerializeField] TargetDetector targetDetectorPrefab;
    #endregion

    private TransferableItemHolder localPlayer;
    private HotPotatoHandler hotPotatoHandler;
    private TrackedWhenOffscreen snitchTracker;
    private PlayersHandler playersHandler;

    private bool initializedTargetDetectors = false;

    #region Appropiate Properties
    private Color CurrentAppropiateUIColor
    {
        get
        {
            switch (hotPotatoHandler.CurrentRoundType)
            {
                case HotPotatoHandler.RoundType.Blessing:
                    return blessingRoundTint;
                case HotPotatoHandler.RoundType.Curse:
                    return curseRoundTint;
                default:
                    return Color.white;
            }
        }
    }
    private Color CurrentAppropiateSliderBackgroundColor
    {
        get
        {
            switch (hotPotatoHandler.CurrentRoundType)
            {
                case HotPotatoHandler.RoundType.Blessing:
                    return blessingSliderBackgroundColor;
                case HotPotatoHandler.RoundType.Curse:
                    return curseSliderBackgroundColor;
                default:
                    return Color.white;
            }
        }
    }
    private Sprite CurrentAppropiateSnitchSprite
    {
        get
        {
            switch (hotPotatoHandler.CurrentRoundType)
            {
                case HotPotatoHandler.RoundType.Blessing:
                    return blessingRoundSnitchSprite;
                case HotPotatoHandler.RoundType.Curse:
                    return curseRoundSnitchSprite;
                default:
                    return null;
            }
        }
    }
    private Sprite CurrentAppropiateTotemSprite
    {
        get
        {
            switch (hotPotatoHandler.CurrentRoundType)
            {
                case HotPotatoHandler.RoundType.Blessing:
                    return blessingTotemSprite;
                case HotPotatoHandler.RoundType.Curse:
                    return curseTotemSprite;
                default:
                    return null;
            }
        }
    }
    private Material CurrentAppropiateJungleFeverMaterial
    {
        get
        {
            switch (hotPotatoHandler.CurrentRoundType)
            {
                case HotPotatoHandler.RoundType.Blessing:
                    return jungleFeverBlessingTextMaterial;
                case HotPotatoHandler.RoundType.Curse:
                    return jungleFeverCurseTextMaterial;
                default:
                    return null;
            }
        }
    }
    private Material CurrentAppropiateJanBradyMaterial
    {
        get
        {
            switch (hotPotatoHandler.CurrentRoundType)
            {
                case HotPotatoHandler.RoundType.Blessing:
                    return janBradyBlessingTextMaterial;
                case HotPotatoHandler.RoundType.Curse:
                    return janBradyCurseTextMaterial;
                default:
                    return null;
            }
        }
    }
    private Sprite CurrentAppropiateSliderFillSprite
    {
        get
        {
            switch (hotPotatoHandler.CurrentRoundType)
            {
                case HotPotatoHandler.RoundType.Blessing:
                    return blessingSliderFill;
                case HotPotatoHandler.RoundType.Curse:
                    return curseSliderFill;
                default:
                    return null;
            }
        }
    }
    #endregion

    private bool LocalPlayerHasSnith => hotPotatoHandler.Snitch?.CurrentHolder == localPlayer && localPlayer != null;

    public void AnimEvt_SwapSnitchSprite()
    {
        roundChangeAnimationTotemImage.sprite = CurrentAppropiateTotemSprite;
        snitchTracker.ViewTransform.GetComponent<Image>().sprite = CurrentAppropiateSnitchSprite;
        if (hotPotatoHandler.Snitch) hotPotatoHandler.Snitch.GetComponent<SpriteRenderer>().sprite = CurrentAppropiateSnitchSprite;
    }

    private void Update()
    {
        if (!localPlayer) InitializationUtilities.FindLocalPlayer(out localPlayer);
        else if (!initializedTargetDetectors && playersHandler.AllPlayersHaveSpawned) InitializeTargetDetectors();

        if (!hotPotatoHandler.SnitchHasActivated)
        {
            timeLeftInRoundSlider.value = hotPotatoHandler.TimeBeforeSnitchActivation;
            if (hotPotatoHandler.TimeBeforeSnitchActivation < shownCountdownLength)
            {
                countdownTextField.gameObject.SetActive(true);
                countdownTextField.text = Mathf.CeilToInt(hotPotatoHandler.TimeBeforeSnitchActivation).ToString();
            }
        }
        else
        {
            timeLeftInRoundSlider.maxValue = hotPotatoHandler.RoundDurationSinceLastReset;
            timeLeftInRoundSlider.value = hotPotatoHandler.TimeLeftInRound;
        }
    }

    private void SetRoundUI(HotPotatoHandler.RoundType roundType)
    {
        //Make sure that the slider is updated
        Update();

        //Tint the relevant UI
        sliderBackgroundImage.color = CurrentAppropiateSliderBackgroundColor;

        foreach (Graphic graphic in graphicsToTintOnRoundChange)
        {
            graphic.color = CurrentAppropiateUIColor;
        }

        foreach (TextMeshProUGUI textField in jungleFeverTextsToTintOnRoundChange)
        {
            textField.fontMaterial = CurrentAppropiateJungleFeverMaterial;
        }

        foreach (TextMeshProUGUI textField in janBradyTextsToTintOnRoundChange)
        {
            textField.fontMaterial = CurrentAppropiateJanBradyMaterial;
        }

        sliderFillImage.sprite = CurrentAppropiateSliderFillSprite;
        sliderFillImage.type = Image.Type.Tiled;

        totemImage.sprite = CurrentAppropiateTotemSprite;

        //Set the counter
        roundCounterTextField.text = $"{hotPotatoHandler.CurrentRoundNumber}/{hotPotatoHandler.TotalRoundCount}";

        //Do the round change animation
        GetComponent<Animator>().Play("anim_RoundChange");

        UpdateMissionText(LocalPlayerHasSnith, roundType);
        UpdateLocalPlayerTargetDetectors(LocalPlayerHasSnith, roundType);
    }

    private void OnSnitchTransfered(TransferableItemHolder oldHolder, TransferableItemHolder newHolder)
    {
        UpdateMissionText(LocalPlayerHasSnith, hotPotatoHandler.CurrentRoundType);
        UpdateLocalPlayerTargetDetectors(LocalPlayerHasSnith, hotPotatoHandler.CurrentRoundType);   
    }

    private void UpdateMissionText(bool localPlayerHasSnitch, HotPotatoHandler.RoundType roundType)
    {
        string message = string.Empty;
        if (roundType == HotPotatoHandler.RoundType.Blessing) message = localPlayerHasSnitch ? msgBlessingHasTotem : msgBlessingDoesntHaveTotem;
        else if (roundType == HotPotatoHandler.RoundType.Curse) message = localPlayerHasSnitch ? msgCurseHasTotem : msgCurseDoesntHaveTotem;

        missionText.SetText(message);
    }

    private void OnSnitchActivated()
    {
        countdownTextField.gameObject.SetActive(false);
        roundCounterTextField.gameObject.SetActive(true);
        totemImage.gameObject.SetActive(true);
        roundCounterLabel.SetActive(true);
        timeLeftInRoundSlider.gameObject.SetActive(true);
    }

    private void UpdateLocalPlayerTargetDetectors(bool localPlayerHasSnitch, HotPotatoHandler.RoundType roundType)
    {
        TargetDetector[] targetDetectors = localPlayer.GetComponentsInChildren<TargetDetector>(true);
        for (int i = 0; i < targetDetectors.Length; i++)
        {
            targetDetectors[i].gameObject.SetActive(localPlayerHasSnitch && roundType == HotPotatoHandler.RoundType.Curse);
        }
    }

    


    #region Initialization
    private void Awake()
    {
        hotPotatoHandler = GetComponent<HotPotatoHandler>();
        playersHandler = GetComponent<PlayersHandler>();
    }

    private void Start()
    {
        hotPotatoHandler.OnNewRound += SetRoundUI;
        hotPotatoHandler.OnSnitchActivated += OnSnitchActivated;

        snitchTracker = hotPotatoHandler.Snitch.GetComponent<TrackedWhenOffscreen>();

        missionText.text = "WAIT FOR THE GEM!";
        timeLeftInRoundSlider.maxValue = hotPotatoHandler.matchStartCountdownLength;
        timeLeftInRoundSlider.minValue = 0;
        timeLeftInRoundSlider.value = hotPotatoHandler.TimeBeforeSnitchActivation;

        countdownTextField.gameObject.SetActive(false);
        roundCounterTextField.gameObject.SetActive(false);
        totemImage.gameObject.SetActive(false);
        roundCounterLabel.SetActive(false);

        timeLeftInRoundSlider.gameObject.SetActive(false);
    }

    private void InitializeTargetDetectors()
    {
        GameObject localPlayer = this.localPlayer.gameObject;
        Camera camera = Camera.main;

        foreach (GameObject player in playersHandler.Players)
        {
            if (player != localPlayer)
            {
                TargetDetector targetDetector = Instantiate(targetDetectorPrefab, localPlayer.transform, false);
                targetDetector.target = player.transform;
                targetDetector.camera = camera;
            }
        }

        initializedTargetDetectors = true;

        UpdateLocalPlayerTargetDetectors(LocalPlayerHasSnith, hotPotatoHandler.CurrentRoundType);
    }

    
    #endregion
}

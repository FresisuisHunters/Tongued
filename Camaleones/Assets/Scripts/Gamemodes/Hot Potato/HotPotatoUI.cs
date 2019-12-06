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
    private PlayersHandler playersHandler;
    private Image offscreenSnitchViewImage;

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
        offscreenSnitchViewImage.sprite = CurrentAppropiateSnitchSprite;
        if (hotPotatoHandler.Snitch) hotPotatoHandler.Snitch.GetComponent<SpriteRenderer>().sprite = CurrentAppropiateSnitchSprite;
    }

    private void SetRoundUI(HotPotatoHandler.RoundType roundType)
    {
        //Make sure the slider is active and updated
        timeLeftInRoundSlider.gameObject.SetActive(true);
        Update();

        //Tint the relevant UI
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
        totemImage.sprite = CurrentAppropiateTotemSprite;

        //Set the counter
        roundCounterTextField.text = $"{hotPotatoHandler.CurrentRoundNumber}/{hotPotatoHandler.TotalRoundCount}";

        //Do the round change animation, except for the first round.
        if (hotPotatoHandler.CurrentRoundNumber > 1) GetComponent<Animator>().Play("anim_RoundChange");

        UpdateMissionText(LocalPlayerHasSnith, roundType);
        UpdateLocalPlayerTargetDetectors(LocalPlayerHasSnith, roundType);
    }

    private void OnSnitchTransfered(TransferableItemHolder oldHolder, TransferableItemHolder newHolder)
    {
        UpdateMissionText(LocalPlayerHasSnith, hotPotatoHandler.CurrentRoundType);
        UpdateLocalPlayerTargetDetectors(LocalPlayerHasSnith, hotPotatoHandler.CurrentRoundType);   
    }
    
    

    private void Update()
    {
        if (!localPlayer) InitializationUtilities.FindLocalPlayer(out localPlayer);
        else if (!initializedTargetDetectors && playersHandler.AllPlayersHaveSpawned) InitializeTargetDetectors();
        

        timeLeftInRoundSlider.maxValue = hotPotatoHandler.RoundDurationSinceLastReset;
        timeLeftInRoundSlider.value = hotPotatoHandler.TimeLeftInRound;
    }

    private void UpdateMissionText(bool localPlayerHasSnitch, HotPotatoHandler.RoundType roundType)
    {
        string message = string.Empty;
        if (roundType == HotPotatoHandler.RoundType.Blessing) message = localPlayerHasSnitch ? msgBlessingHasTotem : msgBlessingDoesntHaveTotem;
        else if (roundType == HotPotatoHandler.RoundType.Curse) message = localPlayerHasSnitch ? msgCurseHasTotem : msgCurseDoesntHaveTotem;

        missionText.SetText(message);
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
        timeLeftInRoundSlider.minValue = 0;
        timeLeftInRoundSlider.gameObject.SetActive(false);

        hotPotatoHandler.OnNewRound += SetRoundUI;
        hotPotatoHandler.OnSnitchTransfered += OnSnitchTransfered;

        offscreenSnitchViewImage = hotPotatoHandler.Snitch.GetComponent<TrackedWhenOffscreen>().ViewTransform.GetComponent<Image>();

        missionText.text = msgBlessingDoesntHaveTotem;
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

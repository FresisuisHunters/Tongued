using UnityEngine;
using UnityEngine.UI;
using TMPro;

#pragma warning disable 649
[RequireComponent(typeof(HotPotatoHandler), typeof(Animator))]
public class HotPotatoUI : MonoBehaviour
{
    [Header("Timer")]
    [SerializeField] private Slider timeLeftInRoundSlider;

    [Header("Round tint")]
    [SerializeField] private Color blessingRoundTint = Color.green;
    [SerializeField] private Color curseRoundTint = Color.red;
    [SerializeField] private Graphic[] graphicsToTintOnRoundChange;
    [SerializeField] private float tintCrossfadeLength = 0.5f;

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


    private TransferableItemHolder localPlayer;
    private HotPotatoHandler hotPotatoHandler;
    private Image offscreenSnitchViewImage;

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


    public void AnimEvt_SwapSnitchSprite()
    {
        roundChangeAnimationTotemImage.sprite = CurrentAppropiateTotemSprite;
        offscreenSnitchViewImage.sprite = CurrentAppropiateSnitchSprite;
        if (hotPotatoHandler.Snitch) hotPotatoHandler.Snitch.GetComponent<SpriteRenderer>().sprite = CurrentAppropiateSnitchSprite;
    }

    private void Update()
    {
        if (!localPlayer) InitializationUtilities.FindLocalPlayer(out localPlayer);

        timeLeftInRoundSlider.maxValue = hotPotatoHandler.RoundDurationSinceLastReset;
        timeLeftInRoundSlider.value = hotPotatoHandler.TimeLeftInRound;
    }

    private void OnNewRound(HotPotatoHandler.RoundType roundType)
    {
        //Make sure the slider is active and updated
        timeLeftInRoundSlider.gameObject.SetActive(true);
        Update();

        //Tint the relevant UI
        Color roundTint = Color.white;
        if (roundType == HotPotatoHandler.RoundType.Blessing) roundTint = blessingRoundTint;
        else if (roundType == HotPotatoHandler.RoundType.Curse) roundTint = curseRoundTint;

        foreach (Graphic graphic in graphicsToTintOnRoundChange)
        {
            graphic.CrossFadeColor(roundTint, tintCrossfadeLength, true, false);
        }

        //Do the round change animation, except for the first round.
        if (hotPotatoHandler.CurrentRoundNumber > 1) GetComponent<Animator>().Play("anim_RoundChange");

        UpdateMissionText();
    }

    private void UpdateMissionText()
    {
        bool localPlayerHasTotem = hotPotatoHandler.Snitch?.CurrentHolder == localPlayer && localPlayer != null;
        HotPotatoHandler.RoundType roundType = hotPotatoHandler.CurrentRoundType;

        string message = string.Empty;
        if (roundType == HotPotatoHandler.RoundType.Blessing) message = localPlayerHasTotem ? msgBlessingHasTotem : msgBlessingDoesntHaveTotem;
        else if (roundType == HotPotatoHandler.RoundType.Curse) message = localPlayerHasTotem ? msgCurseHasTotem : msgCurseDoesntHaveTotem;

        missionText.SetText(message);
    }



    private void Awake()
    {
        hotPotatoHandler = GetComponent<HotPotatoHandler>();
    }

    private void Start()
    {
        timeLeftInRoundSlider.minValue = 0;
        timeLeftInRoundSlider.gameObject.SetActive(false);
        hotPotatoHandler.OnNewRound += OnNewRound;

        hotPotatoHandler.Snitch.OnItemTransfered += (TransferableItemHolder oldHolder, TransferableItemHolder newHolder) => UpdateMissionText();
        offscreenSnitchViewImage = hotPotatoHandler.Snitch.GetComponent<TrackedWhenOffscreen>().ViewTransform.GetComponent<Image>();

        missionText.text = msgBlessingDoesntHaveTotem;
    }
}

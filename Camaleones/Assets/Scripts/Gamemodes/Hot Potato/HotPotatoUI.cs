using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 649
[RequireComponent(typeof(HotPotatoHandler))]
public class HotPotatoUI : MonoBehaviour
{
    [Header("Timer")]
    [SerializeField] private Slider timeLeftInRoundSlider;

    [Header("Round info")]
    [SerializeField] private Color blessingRoundTint = Color.green;
    [SerializeField] private Color curseRoundTint = Color.red;
    [SerializeField] private Graphic[] graphicsToTintOnRoundChange;
    [SerializeField] private float tintCrossfadeLength = 0.5f;

    [Header("Offscreen Snitch View")]
    [SerializeField] private Sprite blessingRoundSnitchImage;
    [SerializeField] private Sprite curseRoundSnitchImage;

    private HotPotatoHandler hotPotatoHandler;
    private Image offscreenSnitchViewImage;


    private void Update()
    {
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

        //Set the offscreen view image
        Sprite offscreenSprite = null;
        if (roundType == HotPotatoHandler.RoundType.Blessing) offscreenSprite = blessingRoundSnitchImage;
        if (roundType == HotPotatoHandler.RoundType.Curse) offscreenSprite = curseRoundSnitchImage;

        offscreenSnitchViewImage.sprite = offscreenSprite;
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

        offscreenSnitchViewImage = hotPotatoHandler.Snitch.GetComponent<TrackedWhenOffscreen>().ViewTransform.GetComponent<Image>();
    }
}

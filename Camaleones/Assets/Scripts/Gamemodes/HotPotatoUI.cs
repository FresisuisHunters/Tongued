using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HotPotatoHandler))]
public class HotPotatoUI : MonoBehaviour
{
    [SerializeField] private Slider timeLeftInRoundSlider;

    [Header("Round info")]
    [SerializeField] private Color blessingRoundTint = Color.green;
    [SerializeField] private Color curseRoundTint = Color.red;
    [SerializeField] private Graphic[] graphicsToTintOnRoundChange;
    [SerializeField] private float tintCrossfadeLength = 0.5f;


    private HotPotatoHandler hotPotatoHandler;


    private void Update()
    {
        timeLeftInRoundSlider.maxValue = hotPotatoHandler.RoundDurationSinceLastReset;
        timeLeftInRoundSlider.value = hotPotatoHandler.TimeLeftInRound;
    }

    private void OnNewRound(HotPotatoHandler.RoundType roundType)
    {
        timeLeftInRoundSlider.gameObject.SetActive(true);
        Update();

        Color roundTint = Color.white;
        if (roundType == HotPotatoHandler.RoundType.Blessing) roundTint = blessingRoundTint;
        else if (roundType == HotPotatoHandler.RoundType.Curse) roundTint = curseRoundTint;

        foreach (Graphic graphic in graphicsToTintOnRoundChange)
        {
            graphic.CrossFadeColor(roundTint, tintCrossfadeLength, true, false);
        }
    }

    private void Awake()
    {
        hotPotatoHandler = GetComponent<HotPotatoHandler>();
    }

    private void Start()
    {
        timeLeftInRoundSlider.minValue = 0;
        hotPotatoHandler.OnNewRound += OnNewRound;

        timeLeftInRoundSlider.gameObject.SetActive(false);
    }
}

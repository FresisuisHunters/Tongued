using UnityEngine;
using UnityEngine.UI;
using Hairibar.Audio.SFX;

[RequireComponent(typeof(OneShotSFXPlayer))]
public class UiSFX : MonoBehaviour
{
    [SerializeField] private SFXClip onClickedSFX;

    private void PlayOnClickedSFX()
    {
        GetComponent<OneShotSFXPlayer>().RequestSFX(onClickedSFX);
    }

    private void Start()
    {
        Button[] buttons = GetComponentsInChildren<Button>(true);
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(PlayOnClickedSFX);
        }

        Toggle[] toggles = GetComponentsInChildren<Toggle>(true);
        foreach (Toggle toggle in toggles)
        {
            toggle.onValueChanged.AddListener((bool active) => { if (active) PlayOnClickedSFX(); });
        }
    }
}

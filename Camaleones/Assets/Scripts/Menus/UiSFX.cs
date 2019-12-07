using UnityEngine;
using UnityEngine.UI;
using Hairibar.Audio.SFX;

#pragma warning disable 649
[RequireComponent(typeof(OneShotSFXPlayer))]
public class UiSFX : MonoBehaviour
{
    [SerializeField] private SFXClip onClickedSFX;

    private OneShotSFXPlayer sfxPlayer;

    public void PlayOnClickedSFX()
    {
        sfxPlayer.RequestSFX(onClickedSFX);
    }

    private void Start()
    {
        sfxPlayer = GetComponent<OneShotSFXPlayer>();

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

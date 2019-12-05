using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

#pragma warning disable 649
[RequireComponent(typeof(CanvasGroup))]
public class SettingsMenuScreen : AMenuScreen
{

    private const string AUDIO_MIXER_MUSIC_KEY = "MusicKey";
    private const string AUDIO_MIXER_SFX_KEY = "SFXVolume";

    [SerializeField] private AudioMixer audioMixer;

    [SerializeField] private ToggleGroup soundOptionsToggleGroup;
    [SerializeField] private Toggle soundOnToggle;
    [SerializeField] private Toggle soundOffToggle;
    [SerializeField] private ToggleGroup musicOptionsToggleGroup;
    [SerializeField] private Toggle musicOnToggle;
    [SerializeField] private Toggle musicOffToggle;

    public override void GoBack()
    {
        MenuManager.SetActiveMenuScreen<MainMenuScreen>();
    }

    private void Awake() {
        soundOnToggle.onValueChanged.AddListener((value) => {
            SetSoundVolume(value);
            UpdateToggle(soundOnToggle, value);
        });
        soundOffToggle.onValueChanged.AddListener((value) => UpdateToggle(soundOffToggle, value));
        
        musicOnToggle.onValueChanged.AddListener((value) => {
            SetMusicVolume(value);
            UpdateToggle(musicOnToggle, value);
        });
        musicOffToggle.onValueChanged.AddListener((value) => UpdateToggle(musicOffToggle, value));
    }

    private void OnEnable() {
        UpdateToggles();
    }

    private void UpdateToggles() {
        UpdateToggle(soundOnToggle, Settings.enableSound);
        UpdateToggle(soundOffToggle, !Settings.enableSound);

        UpdateToggle(musicOnToggle, Settings.enableMusic);
        UpdateToggle(musicOffToggle, !Settings.enableMusic);
    }

    private void SetSoundVolume(bool enable) {
        Settings.enableSound = enable;

        float volume = (enable) ? 0 : -80;
        audioMixer.SetFloat(AUDIO_MIXER_SFX_KEY, volume);
    }

    private void SetMusicVolume(bool enable) {
        Settings.enableMusic = enable;

        float volume = (enable) ? 0 : -80;
        audioMixer.SetFloat(AUDIO_MIXER_MUSIC_KEY, volume);
    }

    private void UpdateToggle(Toggle toggle, bool isOn) {
        toggle.isOn = isOn;

        Image toggleImage = toggle.image;
        toggleImage.color = (toggle.isOn) ? Color.white : Color.grey;
    }

}
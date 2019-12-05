using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

#pragma warning disable 649
[RequireComponent(typeof(CanvasGroup))]
public class SettingsMenuScreen : AMenuScreen
{

    private const string AUDIO_MIXER_MASTER_KEY = "Master";
    private const string AUDIO_MIXER_SFX_KEY = "SFXVolume";

    [SerializeField] private AudioMixer audioMixer;

    [SerializeField] private Toggle soundOnToggle;
    [SerializeField] private Toggle soundOffToggle;
    [SerializeField] private Toggle musicOnToggle;
    [SerializeField] private Toggle musicOffToggle;

    public override void GoBack()
    {
        MenuManager.SetActiveMenuScreen<MainMenuScreen>();
    }

    private void Start() {
        soundOnToggle.onValueChanged.AddListener((value) => SetSoundVolume(value));
        musicOnToggle.onValueChanged.AddListener((value) => SetMusicVolume(value));
    }

    private void OnEnable() {
        soundOnToggle.isOn = Settings.enableSound;
        soundOffToggle.isOn = !Settings.enableSound;
        musicOnToggle.isOn = Settings.enableMusic;
        musicOffToggle.isOn = !Settings.enableMusic;
    }

    private void SetSoundVolume(bool enable) {
        Settings.enableSound = enable;

        float volume = (enable) ? 0 : -80;
        audioMixer.SetFloat(AUDIO_MIXER_SFX_KEY, volume);
    }

    private void SetMusicVolume(bool enable) {
        Settings.enableMusic = enable;

        float volume = (enable) ? 0 : -80;
        audioMixer.SetFloat(AUDIO_MIXER_MASTER_KEY, volume);
    }

}
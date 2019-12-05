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

    [SerializeField] private Toggle soundOnToggle;
    [SerializeField] private Toggle soundOffToggle;
    [SerializeField] private Toggle musicOnToggle;
    [SerializeField] private Toggle musicOffToggle;
    [SerializeField] private Toggle mouseControlToggle;
    [SerializeField] private Toggle touchControlToggle;

    public override void GoBack()
    {
        MenuManager.SetActiveMenuScreen<MainMenuScreen>();
    }

    protected override void OnOpen(System.Type previousScreen) {
        UpdateToggles();
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

        mouseControlToggle.onValueChanged.AddListener((value) => {
            SetControlScheme();
            UpdateToggle(mouseControlToggle, Settings.controlScheme == Settings.ControlScheme.Mouse);
        });
        touchControlToggle.onValueChanged.AddListener((value) => UpdateToggle(touchControlToggle,
            Settings.controlScheme == Settings.ControlScheme.Touch));
    }

    private void UpdateToggles() {
        UpdateToggle(soundOffToggle, !Settings.enableSound);
        UpdateToggle(soundOnToggle, Settings.enableSound);

        UpdateToggle(musicOffToggle, !Settings.enableMusic);
        UpdateToggle(musicOnToggle, Settings.enableMusic);

        UpdateToggle(mouseControlToggle, Settings.controlScheme == Settings.ControlScheme.Mouse);
        UpdateToggle(touchControlToggle, Settings.controlScheme == Settings.ControlScheme.Touch);
    }

    private void SetSoundVolume(bool enable) {
        Settings.enableSound = enable;

        float volume = (enable) ? 0 : -80;
        audioMixer.SetFloat(AUDIO_MIXER_SFX_KEY, volume);
    }

    private void SetMusicVolume(bool enable) {
        Settings.enableMusic = enable;

        float volume = (enable) ? 0 : -80;
        audioMixer.SetFloat(AUDIO_MIXER_MUSIC_KEY, volume); // TODO
    }

    private void SetControlScheme() {
        Settings.ControlScheme controlScheme = (touchControlToggle.isOn) ? Settings.ControlScheme.Touch : Settings.ControlScheme.Mouse;
        Settings.controlScheme = controlScheme;
    }

    private void UpdateToggle(Toggle toggle, bool isOn) {
        toggle.isOn = isOn;

        Image toggleImage = toggle.image;
        toggleImage.color = (toggle.isOn) ? Color.white : Color.grey;
    }

}
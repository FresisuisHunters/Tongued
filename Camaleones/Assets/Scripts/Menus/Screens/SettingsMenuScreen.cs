using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

#pragma warning disable 649
[RequireComponent(typeof(CanvasGroup))]
public class SettingsMenuScreen : AMenuScreen
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Toggle soundOnToggle;
    [SerializeField] private Toggle soundOffToggle;
    [SerializeField] private Toggle musicOnToggle;
    [SerializeField] private Toggle musicOffToggle;
    [SerializeField] private Toggle mouseControlToggle;
    [SerializeField] private TextMeshProUGUI notAvailableText;
    [SerializeField] private Toggle touchControlToggle;

    public override void GoBack()
    {
        Settings.SaveSettings();
        MenuManager.SetActiveMenuScreen<MainMenuScreen>();
    }

    protected override void OnOpen(System.Type previousScreen) {
        UpdateToggles();
    }

    private void Awake() {
        InitializeAudioToggles();
        InitializeControlToggles();
    }

    private void InitializeAudioToggles() {
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

    private void InitializeControlToggles() {
        if (Settings.IsMobilePlatform) {
            mouseControlToggle.enabled = false;
            touchControlToggle.isOn = true;
            notAvailableText.gameObject.SetActive(true);

            Settings.controlScheme = Settings.ControlScheme.Touch;
        } else {
            mouseControlToggle.onValueChanged.AddListener((value) => {
                SetControlScheme();
                UpdateToggle(mouseControlToggle, value);
            });
            touchControlToggle.onValueChanged.AddListener((value) => UpdateToggle(touchControlToggle, value));
        }
    }

    private void UpdateToggles() {
        UpdateToggle(soundOffToggle, !Settings.enableSound);
        UpdateToggle(soundOnToggle, Settings.enableSound);

        UpdateToggle(musicOffToggle, !Settings.enableMusic);
        UpdateToggle(musicOnToggle, Settings.enableMusic);

        UpdateToggle(touchControlToggle, Settings.controlScheme == Settings.ControlScheme.Touch);
        UpdateToggle(mouseControlToggle, Settings.controlScheme == Settings.ControlScheme.Mouse);
    }

    private void SetSoundVolume(bool enable) {
        Settings.SetSoundSettings(audioMixer, enable);
        Settings.SaveSettings();
    }

    private void SetMusicVolume(bool enable) {
        Settings.SetMusicSettings(audioMixer, enable);
        Settings.SaveSettings();
    }

    private void SetControlScheme() {
        Settings.ControlScheme controlScheme = (touchControlToggle.isOn) ? Settings.ControlScheme.Touch : Settings.ControlScheme.Mouse;
        Settings.SetControlSettings(controlScheme);

        Settings.SaveSettings();
    }

    private void UpdateToggle(Toggle toggle, bool isOn) {
        toggle.isOn = isOn;

        Image toggleImage = toggle.image;
        toggleImage.color = (toggle.isOn) ? Color.white : Color.grey;
    }

}
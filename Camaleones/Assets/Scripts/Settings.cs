using UnityEngine;

public class Settings
{
    public enum ControlScheme
    {
        Mouse = 0,
        Touch = 1
    }

    private const string SOUND_KEY = "Sound";
    private const string MUSIC_KEY = "Music";
    private const string CONTROL_SCHEME_KEY = "Control";

    public static readonly bool IS_PHONE = Application.isMobilePlatform;
    public static bool enableSound;
    public static bool enableMusic;
    public static ControlScheme controlScheme;

    static Settings() {
        int soundPlayerPrefs = PlayerPrefs.GetInt(SOUND_KEY, 1);
        enableSound = soundPlayerPrefs == 1;

        int musicPlayerPrefs = PlayerPrefs.GetInt(MUSIC_KEY, 1);
        enableMusic = musicPlayerPrefs == 1;

        if (IS_PHONE) {
            controlScheme = ControlScheme.Touch;
        } else {
            int controlSchemePlayerPrefs = PlayerPrefs.GetInt(CONTROL_SCHEME_KEY, (int) ControlScheme.Mouse);
            controlScheme = (controlSchemePlayerPrefs == 0) ? ControlScheme.Mouse : ControlScheme.Touch;
        }
    }

    public static void SaveSettings() {
        int soundPlayerPrefs = (enableSound) ? 1 : 0;
        PlayerPrefs.SetInt(SOUND_KEY, soundPlayerPrefs);

        int musicPlayerPrefs = (enableMusic) ? 1 : 0;
        PlayerPrefs.SetInt(MUSIC_KEY, musicPlayerPrefs);

        PlayerPrefs.SetInt(CONTROL_SCHEME_KEY, (int) controlScheme);

        PlayerPrefs.Save();
    }

}

﻿using UnityEngine;
using UnityEngine.Audio;

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

    private const string AUDIO_MIXER_SFX_KEY = "SFXVolume";
    private const string AUDIO_MIXER_MUSIC_KEY = "MusicVolume";

    public static Platform platform;
    private static Platform? debugOverridePlatform = null;
    public static bool enableSound;
    public static bool enableMusic;
    public static ControlScheme controlScheme;

    public static bool IsMobilePlatform => platform == Platform.MobileApp || platform == Platform.MobileWebGL;

    static Settings() {
        int soundPlayerPrefs = PlayerPrefs.GetInt(SOUND_KEY, 1);
        enableSound = soundPlayerPrefs == 1;

        int musicPlayerPrefs = PlayerPrefs.GetInt(MUSIC_KEY, 1);
        enableMusic = musicPlayerPrefs == 1;

        RuntimePlatform runtimePlatform = Application.platform;
        if (runtimePlatform == RuntimePlatform.WebGLPlayer) platform = Application.isMobilePlatform ? Platform.MobileWebGL : Platform.DesktopWebGL;
        else platform = Application.isMobilePlatform ? Platform.MobileApp : Platform.DesktopStandalone;

        if (debugOverridePlatform != null) platform = (Platform) debugOverridePlatform;

        if (IsMobilePlatform) {
            controlScheme = ControlScheme.Touch;
        } else {
            int controlSchemePlayerPrefs = PlayerPrefs.GetInt(CONTROL_SCHEME_KEY, (int) ControlScheme.Mouse);
            controlScheme = (controlSchemePlayerPrefs == 0) ? ControlScheme.Mouse : ControlScheme.Touch;
        }
    }

    public static void LoadSettings(AudioMixer audioMixer) {
        SetSoundSettings(audioMixer, enableSound);
        SetMusicSettings(audioMixer, enableMusic);
        SetControlSettings(controlScheme);
    }

    public static void SetSoundSettings(AudioMixer audioMixer, bool enable) {
        enableSound = enable;

        float volume = (enable) ? 0 : -80;
        audioMixer.SetFloat(AUDIO_MIXER_SFX_KEY, volume);
    }

    public static void SetMusicSettings(AudioMixer audioMixer, bool enable) {
        enableMusic = enable;

        float volume = (enable) ? 0 : -80;
        audioMixer.SetFloat(AUDIO_MIXER_MUSIC_KEY, volume);
    }

    public static void SetControlSettings(ControlScheme controlScheme) {
        Settings.controlScheme = controlScheme;
    }

    public static void SaveSettings() {
        int soundPlayerPrefs = (enableSound) ? 1 : 0;
        PlayerPrefs.SetInt(SOUND_KEY, soundPlayerPrefs);

        int musicPlayerPrefs = (enableMusic) ? 1 : 0;
        PlayerPrefs.SetInt(MUSIC_KEY, musicPlayerPrefs);

        PlayerPrefs.SetInt(CONTROL_SCHEME_KEY, (int) controlScheme);

        PlayerPrefs.Save();
    }

    [System.Serializable]
    public enum Platform
    {
        DesktopStandalone,
        DesktopWebGL,
        MobileWebGL,
        MobileApp
    }
}

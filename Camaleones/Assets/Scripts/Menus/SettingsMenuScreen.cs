using System;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 649
[RequireComponent(typeof(CanvasGroup))]
public class SettingsMenuScreen : AMenuScreen
{
    public const string CONTROL_SCHEME_PREF_KEY = "ControlScheme";

    [SerializeField] private Toggle touchControlToggle;
    [SerializeField] private Toggle mouseControlToggle;

    private ToggleGroup controlToggleGroup;

    protected override void OnOpen(Type previousScreen)
    {
        EnableCorrectControlSchemeToggle();

        touchControlToggle.onValueChanged.AddListener(OnTouchControlValueChange);
        mouseControlToggle.onValueChanged.AddListener(OnMouseControlValueChange);
    }

    protected override void OnClose(Type nextScreen)
    {
        touchControlToggle.onValueChanged.RemoveListener(OnTouchControlValueChange);
        mouseControlToggle.onValueChanged.RemoveListener(OnMouseControlValueChange);
    }

    public override void GoBack()
    {
        MenuManager.SetActiveMenuScreen<MainMenuScreen>();
    }

    #region Control Config
    public void OnTouchControlValueChange(bool selected)
    {
        if (selected) SetControlScheme(ControlScheme.Touch);
    }

    public void OnMouseControlValueChange(bool selected)
    {
        if (selected) SetControlScheme(ControlScheme.Mouse);
    }

    private void SetControlScheme(ControlScheme controlScheme)
    {
        PlayerPrefs.SetInt(CONTROL_SCHEME_PREF_KEY, (int)controlScheme);
        PlayerPrefs.Save();
    }

    private void EnableCorrectControlSchemeToggle()
    {
        ControlScheme selectedControlScheme = (ControlScheme) PlayerPrefs.GetInt(CONTROL_SCHEME_PREF_KEY);

        controlToggleGroup.allowSwitchOff = true;
        controlToggleGroup.SetAllTogglesOff();

        switch (selectedControlScheme)
        {
            case ControlScheme.Touch:
                SetControlScheme(ControlScheme.Touch);
                touchControlToggle.isOn = true;
                break;
            case ControlScheme.Mouse:
                SetControlScheme(ControlScheme.Mouse);
                mouseControlToggle.isOn = true;
                break;
        }

        controlToggleGroup.allowSwitchOff = false;
    }

    private void AutoDetectControlScheme()
    {
#if UNITY_STANDALONE || UNITY_EDITOR
        SetControlScheme(ControlScheme.Mouse);
#elif UNITY_WEBGL
        //TODO: Detectar automáticamente si es móvil o desktop.
        //https://forum.unity.com/threads/how-to-detect-if-a-mobile-is-running-the-webgl-scene.440344/
        SetControlScheme(ControlScheme.Touch);
#else
        SetControlScheme(ControlScheme.Touch);
#endif
    }
    #endregion

    private void Awake()
    {
        controlToggleGroup = GetComponentInChildren<ToggleGroup>(true);
        if (!PlayerPrefs.HasKey(CONTROL_SCHEME_PREF_KEY)) AutoDetectControlScheme();
    }
}
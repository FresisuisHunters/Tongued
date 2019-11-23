using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class SettingsMenuScreen : AMenuScreen
{
    public const string CONTROL_SCHEME_PREF_KEY = "ControlScheme";

    public Toggle touchControlToggle;
    public Toggle mouseControlToggle;

    

    protected override void OnOpen(Type previousScreen)
    {
        if (PlayerPrefs.HasKey(CONTROL_SCHEME_PREF_KEY))
        {
            EnableCorrectControlSchemeToggle();
        }
        else
        {
            
        }

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

        switch (selectedControlScheme)
        {
            case ControlScheme.Touch:
                touchControlToggle.group.NotifyToggleOn(touchControlToggle);
                break;
            case ControlScheme.Mouse:
                mouseControlToggle.group.NotifyToggleOn(mouseControlToggle);
                break;
        }
    }

    private void AutoDetectControlScheme()
    {

    }
    #endregion

    private void Start()
    {
        touchControlToggle.onValueChanged.AddListener(OnTouchControlValueChange);
        mouseControlToggle.onValueChanged.AddListener(OnMouseControlValueChange);
    }
}
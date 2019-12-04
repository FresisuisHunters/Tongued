using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class CreditsMenuScren : AMenuScreen
{
    public override void GoBack()
    {
        MenuManager.SetActiveMenuScreen<MainMenuScreen>();
    }
}

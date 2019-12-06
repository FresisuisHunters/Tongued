using UnityEngine;

public class Settings
{
    public enum ControlScheme
    {
        Touch,
        Mouse
    }

    public static readonly bool IS_PHONE = Application.isMobilePlatform;
    public static bool enableSound = true;
    public static bool enableMusic = true;
    public static ControlScheme controlScheme = (Application.isMobilePlatform) ? ControlScheme.Touch : ControlScheme.Mouse;

}

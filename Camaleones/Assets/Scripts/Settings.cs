using UnityEngine;

public class Settings
{

    public enum ControlScheme
    {
        Touch = 0, 
        Mouse = 1
    }

    public static bool enableSound = true;
    public static bool enableMusic = true;
    public static ControlScheme controlScheme = (Application.isMobilePlatform) ? ControlScheme.Touch : ControlScheme.Mouse;

}

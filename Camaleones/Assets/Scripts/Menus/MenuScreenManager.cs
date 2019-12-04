using UnityEngine;

#pragma warning disable 649
public class MenuScreenManager : MonoBehaviour
{
    public const string CONTROL_SCHEME_PREF_KEY = "ControlScheme";

    public AMenuScreen startingMenuScreen;

    private AMenuScreen[] menuScreens;
    private AMenuScreen currentActiveMenuScreen;


    public void SetActiveMenuScreen<T>() where T : AMenuScreen
    {
        for (int i = 0; i < menuScreens.Length; i++)
        {
            if (menuScreens[i] is T)
            {
                SetActiveMenuScreen(menuScreens[i]);
                return;
            }
        }
        
        Debug.LogError($"No Menu Screen \"{typeof(T).FullName}\" was found.");
    }

    public void SetActiveMenuScreen(AMenuScreen menuScreen)
    {
        AMenuScreen previousScreen = currentActiveMenuScreen;
        if (previousScreen) previousScreen.Close(menuScreen.GetType());

        currentActiveMenuScreen = menuScreen;
        menuScreen.Open(previousScreen?.GetType());
    }
    

    private void Awake()
    {
        menuScreens = GetComponentsInChildren<AMenuScreen>(true);
        for (int i = 0; i < menuScreens.Length; i++)
        {
            menuScreens[i].Initialize(this);
        }

        SetControls();
    }

    private void SetControls() {
        int controlScheme = (Application.isMobilePlatform) ? (int)ControlScheme.Touch : (int)ControlScheme.Mouse;
        PlayerPrefs.SetInt(CONTROL_SCHEME_PREF_KEY, controlScheme);
        PlayerPrefs.Save();
    }

    private void Start()
    {
        SetActiveMenuScreen(startingMenuScreen);
    }
}

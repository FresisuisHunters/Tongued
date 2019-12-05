using UnityEngine;

#pragma warning disable 649
public class MenuScreenManager : MonoBehaviour
{
    public AMenuScreen startingMenuScreen;

    private AMenuScreen[] menuScreens;
    private AMenuScreen currentActiveMenuScreen;


    public void SetActiveMenuScreen<T>() where T : AMenuScreen
    {
        for (int i = 0; i < menuScreens.Length; i++)
        {
            if (menuScreens[i] && menuScreens[i] is T)
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
    
    public void GoBack()
    {
        currentActiveMenuScreen.GoBack();
    }

    private void Awake()
    {
        InitializeMenuScreens();
    }

    public void InitializeMenuScreens()
    {
        menuScreens = GetComponentsInChildren<AMenuScreen>(true);
        for (int i = 0; i < menuScreens.Length; i++)
        {
            menuScreens[i].Initialize(this);
        }
    }

    private void Start()
    {
        SetActiveMenuScreen(startingMenuScreen);
    }
}

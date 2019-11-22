using UnityEngine;

#pragma warning disable 649
public class MenuScreenManager : MonoBehaviour
{
    [SerializeField] private AMenuScreen startingMenuScreen;

    private AMenuScreen[] menuScreens;
    private AMenuScreen currentActiveMenuScreen;


    public void SetActiveMenuScreen<T>() where T : AMenuScreen
    {
        for (int i = 0; i < menuScreens.Length; i++)
        {
            if (menuScreens[i] is T)
            {
                SetActiveMenuScreen(menuScreens[i]);
                break;
            }
        }
        
        Debug.LogError($"No Menu Screen \"{typeof(T).FullName}\" was found.");
    }

    public void SetActiveMenuScreen(AMenuScreen menuScreen)
    {
        currentActiveMenuScreen?.Close();
        currentActiveMenuScreen = menuScreen;
        menuScreen.Open();
    }


    private void Start()
    {
        menuScreens = GetComponentsInChildren<AMenuScreen>(true);
        for (int i = 0; i < menuScreens.Length; i++)
        {
            menuScreens[i].Initialize(this);
        }

        SetActiveMenuScreen(startingMenuScreen);
    }
}

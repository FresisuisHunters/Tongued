using UnityEngine;

/// <summary>
/// Base para pantallas de menú.
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public abstract class AMenuScreen : MonoBehaviour
{
    protected MenuScreenManager MenuManager { get; private set; }
    private CanvasGroup canvasGroup;

    public void Open(System.Type previousScreen)
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        OnOpen(previousScreen);
    }

    public void Close(System.Type nextScreen)
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        OnClose(nextScreen);
    }

    public virtual void GoBack()
    {
        Debug.LogWarning("This screen has no Back function.", this);
    }

    protected virtual void OnOpen(System.Type previousScreen) { }
    protected virtual void OnClose(System.Type nextScreen) { }

    

    /// <summary>
    /// Inicializador, sólo MenuScreenManager debería llamarlo.
    /// </summary
    public void Initialize(MenuScreenManager menuScreenManager)
    {
        MenuManager = menuScreenManager;
        canvasGroup = GetComponent<CanvasGroup>();

        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        gameObject.SetActive(true);
    }
}

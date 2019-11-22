using UnityEngine;

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

    protected abstract void OnOpen(System.Type previousScreen);
    protected abstract void OnClose(System.Type nextScreen);

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

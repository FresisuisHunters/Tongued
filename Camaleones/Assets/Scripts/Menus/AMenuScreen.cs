using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public abstract class AMenuScreen : MonoBehaviour
{
    protected MenuScreenManager MenuManager { get; private set; }

    private CanvasGroup canvasGroup;

    public void Open()
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        OnOpen();
    }

    public void Close()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        OnClose();
    }

    public abstract void OnOpen();
    public abstract void OnClose();

    public void Initialize(MenuScreenManager menuScreenManager)
    {
        MenuManager = menuScreenManager;
        canvasGroup = GetComponent<CanvasGroup>();
    }
}

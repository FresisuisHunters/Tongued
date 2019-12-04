using UnityEngine;
using UnityEngine.UI;
using System.Linq;

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

        SetInteractable(true);

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

    protected void SetInteractable(bool interactable, params Selectable[] exceptions)
    {
        Selectable[] selectables = GetComponentsInChildren<Selectable>();
        for (int i = 0; i < selectables.Length; i++)
        {
            if (exceptions != null && !exceptions.Contains(selectables[i]))
            {
                selectables[i].interactable = interactable;
            }
        }
    }
    

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

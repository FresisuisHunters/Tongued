using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Botón de ir atrás en MenuScreens. Configura el evento del botón automáticamente.
/// </summary>
[RequireComponent(typeof(Button))]
public class BackButton : MonoBehaviour
{
    public void GoBack()
    {
        AMenuScreen menuScreen = GetComponentInParent<AMenuScreen>();
        if (menuScreen) menuScreen.GoBack();
        else Debug.LogError("BackButton no éstá dentro de ningún AMenuScreen.", this);
    }

    private void Start()
    {
        UnityEvent onClick = GetComponent<Button>().onClick;
        //Por si alguien ha puesto manualmente nuestros listeners, los quitamos antes de suscribirnos automáticamente.
        for (int i = 0; i < onClick.GetPersistentEventCount(); i++)
        {
            if (onClick.GetPersistentTarget(i) == this)
            {
                onClick.SetPersistentListenerState(i, UnityEventCallState.Off);
            }
        }

        onClick.AddListener(GoBack);
    }
}

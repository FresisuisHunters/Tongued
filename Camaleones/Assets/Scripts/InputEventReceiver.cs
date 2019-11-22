using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Utilidad para recibir eventos de input en cualquier punto de la pantalla.
/// Utilizamos EventSystems en vez de Input porque abstrae detalles de ratón vs pantalla táctil, y porque da eventos con información útil como la cámara desde la que se ha dado el evento.
/// Estos eventos los reciben objetos sólo cuando ocurren sobre ellos (como hacer click en un objeto).
/// Este componente va junto a un canvas que escala para llenar el frustrum, recibiendo así eventos en cualquier punto de la pantalla.
/// Permite a cualquier objeto subscribirse a estos eventos.
/// </summary>
[RequireComponent(typeof(Canvas), typeof(EventSystem)), RequireComponent(typeof(GraphicRaycaster), typeof(Image))]
public class InputEventReceiver : EventTrigger
{ 
    public void AddListener(EventTriggerType type, UnityAction<BaseEventData> callback)
    {
        Entry entry = new Entry();
        entry.eventID = type;
        entry.callback.AddListener((data) => { callback(data); });
        triggers.Add(entry);
    }

    private void Awake()
    {
        GetComponent<Image>().color = new Color(0, 0, 0, 0);
        Canvas canvas = GetComponent<Canvas>();

        canvas.renderMode = RenderMode.ScreenSpaceCamera;
    }
}

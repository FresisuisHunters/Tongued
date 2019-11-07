using UnityEngine.Events;
using UnityEngine.EventSystems;

public class InputEventReceiver : EventTrigger
{ 
    public void AddListener(EventTriggerType type, UnityAction<BaseEventData> callback)
    {
        Entry entry = new Entry();
        entry.eventID = type;
        entry.callback.AddListener((data) => { callback(data); });
        triggers.Add(entry);
    }
}

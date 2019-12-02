using UnityEngine;

/// <summary>
/// Hace que el AttachedStruggler del gancho que nos ha enganchado suelte a este objeto tras un tiempo.
/// </summary>
public class ResistBeingHooked : MonoBehaviour, IOnHookedListener
{
    public float detachCountdownLength = 1;

    void IOnHookedListener.OnHooked(Vector2 pullDirection, Hook hook)
    {
        hook.GetComponent<AttachedStruggler>().DetachAfterCountdown(detachCountdownLength);
    }
}

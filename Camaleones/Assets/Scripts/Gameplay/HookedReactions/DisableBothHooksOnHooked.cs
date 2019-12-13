using UnityEngine;

[RequireComponent(typeof(HookHeadCollisionDetector))]
public class DisableBothHooksOnHooked : MonoBehaviour, IOnHookedListener
{
    private Hook hook;

    void IOnHookedListener.OnHooked(Vector2 pullDirection, Hook hookThatHookedMe)
    {
        hookThatHookedMe.Disable();
        hook.Disable();
    }

    private void Awake()
    {
        hook = GetComponentInParent<Hook>();
    }
}

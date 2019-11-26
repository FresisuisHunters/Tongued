using System.Collections;
using UnityEngine;

/// <summary>
/// Implementa el forcejeo que hay cuando enganchas a un objeto con ResistBeingHooked.
/// </summary>
[RequireComponent(typeof(Hook))]
public class AttachedStruggler : MonoBehaviour
{
    private Hook hook;


    public void DetachAfterCountdown(float countdownLength)
    {
        StartCoroutine(_DisableHookAfterCountdown(countdownLength));
    }


    private IEnumerator _DisableHookAfterCountdown(float countdownLength)
    {
        yield return new WaitForSeconds(countdownLength);
        //TODO: Alguna animación aquí. Tintar su color hacia el rojo?
        hook.Disable();
    }

    private void CancelCountdown()
    {
        StopAllCoroutines();
    }

    private void Awake()
    {
        hook = GetComponent<Hook>();
        hook.OnDisabled += CancelCountdown;
    }
}

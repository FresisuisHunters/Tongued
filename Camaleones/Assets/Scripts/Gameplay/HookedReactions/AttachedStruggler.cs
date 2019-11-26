using System.Collections;
using UnityEngine;

/// <summary>
/// Implementa el forcejeo que hay cuando enganchas a un objeto con ResistBeingHooked.
/// </summary>
[RequireComponent(typeof(Hook))]
public class AttachedStruggler : MonoBehaviour
{
    public Color finalTongueTint;

    private Hook hook;
    private LineRenderer lineRenderer;

    public void DetachAfterCountdown(float countdownLength)
    {
        StartCoroutine(_DisableHookAfterCountdown(countdownLength));
    }


    private IEnumerator _DisableHookAfterCountdown(float countdownLength)
    {
        float t = 0;
        Color white = Color.white;
        Color lerpedColor;
        do
        {
            lerpedColor = Color.Lerp(white, finalTongueTint, t / countdownLength);
            lineRenderer.startColor = lerpedColor;
            lineRenderer.endColor = lerpedColor;
            t += Time.deltaTime;

            yield return null;
        } while (t < countdownLength);
        
        hook.Disable();
        ResetTint();
    }

    private void CancelCountdown()
    {
        StopAllCoroutines();
        ResetTint();
    }

    private void ResetTint()
    {
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.white;
    }

    private void Awake()
    {
        hook = GetComponent<Hook>();
        hook.OnDisabled += CancelCountdown;

        lineRenderer = GetComponentInChildren<LineRenderer>();
    }
}

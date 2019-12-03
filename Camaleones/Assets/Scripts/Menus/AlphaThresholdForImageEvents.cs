using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class AlphaThresholdForImageEvents : MonoBehaviour
{
    [SerializeField, Range(0, 1)] private float alphaThreshold = 0.1f;

    private void Start()
    {
        Apply();
    }

    private void OnValidate()
    {
        Apply();
    }

    private void Apply()
    {
        GetComponent<Image>().alphaHitTestMinimumThreshold = alphaThreshold;
    }

}

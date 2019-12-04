using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinRotation : MonoBehaviour
{
    public float amplitude;
    public float frequency;

    private float yCoord;
    private float baseRotation;

    
    private void Update()
    {
        float offset = (Mathf.PerlinNoise(Time.time * frequency, yCoord) * 2 - 1) * amplitude;
        float newRotation = baseRotation + offset;

        transform.localRotation = Quaternion.Euler(0, 0, newRotation);
    }

    private void Start()
    {
        baseRotation = transform.localRotation.eulerAngles.z;
        yCoord = Random.Range(0, 10000f);
    }
}

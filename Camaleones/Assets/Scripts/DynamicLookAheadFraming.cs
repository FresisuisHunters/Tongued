using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class DynamicLookAheadFraming : MonoBehaviour
{
    public Rigidbody2D trackedRigidbody;
    public float thresholdVelocity = 5;
    [Range(0, 0.5f)] public float lookaheadAmount = 0.2f;

    private new CinemachineFramingTransposer camera;

    private void Update()
    {
        Vector2 velocity = trackedRigidbody.velocity;

        if (velocity.x > thresholdVelocity) camera.m_ScreenX = 0.5f - lookaheadAmount;

        if (Mathf.Abs(velocity.x) > thresholdVelocity) camera.m_ScreenX = 0.5f - lookaheadAmount * Mathf.Sign(velocity.x);
        else camera.m_ScreenX = 0.5f;

        /*
        if (Mathf.Abs(velocity.y) > thresholdVelocity) camera.m_ScreenY = 0.5f + lookaheadAmount * Mathf.Sign(velocity.y);
        else camera.m_ScreenY = 0.5f;
        */
    }


    private void Start()
    {
        camera = GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineFramingTransposer>();
    }

}

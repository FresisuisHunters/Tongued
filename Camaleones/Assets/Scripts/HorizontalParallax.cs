using UnityEngine;

public class HorizontalParallax : MonoBehaviour
{
    public float velocityFactor = 0.1f;

    //private new Camera camera;
    private Transform cameraTransform;
    private float lastCameraPos;


    private void Update()
    {
        float newCameraPos = cameraTransform.position.x;
        float worldSpaceCameraDelta = newCameraPos - lastCameraPos;

        transform.position += new Vector3(worldSpaceCameraDelta * velocityFactor, 0, 01);
        lastCameraPos = newCameraPos;
    }

    private void Start()
    {
        cameraTransform = Camera.main.transform;
    }
}

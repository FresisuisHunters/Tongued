using UnityEngine;
using UnityEngine.UI;

public class UVParallax : MonoBehaviour
{
    public float velocityFactor = 0.1f;

    private Material material;
    private RectTransform rectTransform;

    private new Camera camera;
    private Transform cameraTransform;
    private Vector2 lastCameraPos;
    

    private void Update()
    {
        Vector2 newCameraPos = cameraTransform.position;
        Vector2 worldSpaceCameraDelta = newCameraPos - lastCameraPos;

        float width = Vector3.Distance(camera.ViewportToWorldPoint(Vector3.zero), camera.ViewportToWorldPoint(new Vector3(1, 0, 0)));
        float height = Vector3.Distance(camera.ViewportToWorldPoint(Vector3.zero), camera.ViewportToWorldPoint(new Vector3(0, 1, 0)));

        Vector2 uvSpaceParallaxDelta = new Vector2(worldSpaceCameraDelta.x / width, worldSpaceCameraDelta.y / height) * velocityFactor;
        material.mainTextureOffset = material.mainTextureOffset + uvSpaceParallaxDelta;        

        lastCameraPos = newCameraPos;
    }


    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        camera = Camera.main;
        cameraTransform = camera.transform;

        Image image = GetComponent<Image>();
        material = new Material(image.material);
        image.material = material;
    }
}

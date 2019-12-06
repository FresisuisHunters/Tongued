using UnityEngine;

#pragma warning disable 649
[RequireComponent(typeof(SpriteRenderer))]
public class TargetDetector : MonoBehaviour
{
    [SerializeField] private Vector2 baseFacing;
    
    [System.NonSerialized] public Transform target;
    [System.NonSerialized] public new Camera camera;

    private SpriteRenderer spriteRenderer;


    private void Update()
    {
        if (!target)
        {
            spriteRenderer.enabled = false;
            return;
        }

        bool targetIsOnScreen = false;
        if (camera)
        {
            Vector2 viewportPos = camera.WorldToViewportPoint(target.position);
            targetIsOnScreen = viewportPos.x > 0 && viewportPos.x < 1 && viewportPos.y > 0 && viewportPos.y < 1;
        }
        
        if (targetIsOnScreen) spriteRenderer.enabled = false;
        else
        {
            spriteRenderer.enabled = true;
            float angle = Vector2.SignedAngle(baseFacing, target.position - transform.position);
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
}

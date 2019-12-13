using UnityEngine;

#pragma warning disable 649
public class DestroyDependingOnPlatform : MonoBehaviour
{
    [SerializeField] private Settings.Platform[] allowedInPlatforms;

    private void Awake()
    {
        bool isAllowed = false;

        for (int i = 0; i < allowedInPlatforms.Length; i++)
        {
            if (allowedInPlatforms[i] == Settings.platform) isAllowed = true;
        }

        if (isAllowed) Destroy(this);
        else Destroy(gameObject);
    }
}

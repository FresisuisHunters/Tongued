using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyDependingOnPlatform : MonoBehaviour
{
    [SerializeField] private PlatformSetting setting;

    private void Awake()
    {
        if ((Settings.IS_PHONE && setting == PlatformSetting.DestroyIfMobile) ||
            (!Settings.IS_PHONE && setting == PlatformSetting.DestroyIfDesktop)) Destroy(gameObject);
        else Destroy(this);
    }

    [System.Serializable]
    private enum PlatformSetting
    {
        DestroyIfMobile,
        DestroyIfDesktop
    }
}

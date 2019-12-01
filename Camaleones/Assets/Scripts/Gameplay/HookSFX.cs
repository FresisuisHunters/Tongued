using UnityEngine;
using Hairibar.Audio.SFX;

#pragma warning disable 649
[RequireComponent(typeof(Hook), typeof(OneShotSFXPlayer))]
public class HookSFX : MonoBehaviour
{
    [SerializeField] private SFXClip attachToTerrainSFX;
    [SerializeField] private SFXClip attachToEntitySFX;

    private OneShotSFXPlayer sfxPlayer;

    private void OnHookAtached(Hook.AttachPointType attachPointType)
    {
        switch (attachPointType)
        {
            case Hook.AttachPointType.Terrain:
                sfxPlayer.RequestSFX(attachToTerrainSFX);
                break;
            case Hook.AttachPointType.Entity:
                sfxPlayer.RequestSFX(attachToEntitySFX);
                break;
        }
    }

    private void Start()
    {
        sfxPlayer = GetComponent<OneShotSFXPlayer>();

        Hook hook = GetComponent<Hook>();
        hook.OnAttached += OnHookAtached;
    }
}

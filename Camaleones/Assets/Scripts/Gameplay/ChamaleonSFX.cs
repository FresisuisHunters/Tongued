using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hairibar.Audio.SFX;

#pragma warning disable 649
[RequireComponent(typeof(SFXPlayer), typeof(HookThrower), typeof(Rigidbody2D))]
public class ChamaleonSFX : MonoBehaviour
{
    [SerializeField] private SFXClip throwHookSFX;
    [SerializeField] private SFXClip disableHookSFX;

    private SFXPlayer sfxPlayer;


    private void Awake()
    {
        sfxPlayer = GetComponent<SFXPlayer>();
    }

    private void Start()
    {
        HookThrower hookThrower = GetComponent<HookThrower>();

        hookThrower.OnHookThrown += (Vector2 target) => sfxPlayer.RequestSFX(throwHookSFX);
        hookThrower.OnHookDisabled += () => sfxPlayer.RequestSFX(disableHookSFX);
    }
}

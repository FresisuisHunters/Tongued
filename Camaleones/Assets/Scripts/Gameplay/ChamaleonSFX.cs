using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hairibar.Audio.SFX;

#pragma warning disable 649
[RequireComponent(typeof(OneShotSFXPlayer), typeof(HookThrower), typeof(Rigidbody2D)), RequireComponent(typeof(RepeatAtFrequencySFXPlayer))]
public class ChamaleonSFX : MonoBehaviour
{
    [Header("Retract")]
    [SerializeField] private float retractSFXMaxFrequency = 10;
    [SerializeField] private SFXClip retractSFX;

    [Header("Others")]
    [SerializeField] private SFXClip throwHookSFX;
    [SerializeField] private SFXClip disableHookSFX;

    private HookThrower hookThrower;
    private OneShotSFXPlayer sfxPlayer;
    private RepeatAtFrequencySFXPlayer retractPlayer;

    private float retractAmountThisFrame;

    private void LateUpdate()
    {
        retractPlayer.enabled = retractAmountThisFrame > 0;
        retractPlayer.frequency = retractSFXMaxFrequency * retractAmountThisFrame;
        retractAmountThisFrame = 0;
    }

    
    private void Awake()
    {
        sfxPlayer = GetComponent<OneShotSFXPlayer>();
        retractPlayer = GetComponent<RepeatAtFrequencySFXPlayer>();
        retractPlayer.sfx = retractSFX;
    }

    private void Start()
    {
        hookThrower = GetComponent<HookThrower>();

        hookThrower.OnHookThrown += (Vector2 target) => sfxPlayer.RequestSFX(throwHookSFX);
        hookThrower.OnHookDisabled += () => sfxPlayer.RequestSFX(disableHookSFX);
        hookThrower.OnRetracted += (float amount) => retractAmountThisFrame = amount;
    }
}

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

    [Header("Weee")]
    [SerializeField] private SFXClip weeeSFX;
    [SerializeField] private float minVelocityForWeeeSFX = 10;
    [SerializeField, Range(0, 1)] private float weeeSFXChance = 0.3f;

    [Header("Others")]
    [SerializeField] private SFXClip throwHookSFX;
    [SerializeField] private SFXClip disableHookSFX;

    private new Rigidbody2D rigidbody;
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

    private void OnHookDisabled()
    {
        if (rigidbody.velocity.magnitude > minVelocityForWeeeSFX && Random.value < weeeSFXChance)
        {
            sfxPlayer.RequestSFX(weeeSFX);
        }
        else
        {
            sfxPlayer.RequestSFX(disableHookSFX);
        }
    }
    
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        hookThrower = GetComponent<HookThrower>();

        sfxPlayer = GetComponent<OneShotSFXPlayer>();
        retractPlayer = GetComponent<RepeatAtFrequencySFXPlayer>();
        retractPlayer.sfx = retractSFX;
    }

    private void Start()
    {
        hookThrower.OnHookThrown += (Vector2 target) => sfxPlayer.RequestSFX(throwHookSFX);
        hookThrower.OnHookDisabled += OnHookDisabled;
        hookThrower.OnRetracted += (float amount) => retractAmountThisFrame = amount;
    }
}

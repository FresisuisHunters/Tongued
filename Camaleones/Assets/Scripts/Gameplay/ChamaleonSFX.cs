using UnityEngine;
using UnityEngine.Audio;
using Hairibar.Audio.SFX;

#pragma warning disable 649
[RequireComponent(typeof(OneShotSFXPlayer), typeof(HookThrower), typeof(Rigidbody2D)), RequireComponent(typeof(RepeatAtFrequencySFXPlayer))]
public class ChamaleonSFX : MonoBehaviour
{
    #region Inspector
    [Header("Retract")]
    [SerializeField] private float retractSFXMaxFrequency = 10;
    [SerializeField] private SFXClip retractSFX;

    [Header("Weee")]
    [SerializeField] private SFXClip weeeSFX;
    [SerializeField] private float minVelocityForWeeeSFX = 10;
    [SerializeField, Range(0, 1)] private float weeeSFXChance = 0.3f;

    [Header("Water")]
    [SerializeField] private SFXClip localSplashIn;
    [SerializeField] private SFXClip remoteSplashIn;
    [SerializeField] private SFXClip splashOut;
    [SerializeField] private AudioMixerSnapshot underwaterSnapshot;
    [SerializeField] private AudioMixerSnapshot normalGameplaySnapshot;
    [SerializeField] private float snapshotTransitionLength;
    [SerializeField] private float minVelocityForSplash = 5;

    [Header("Others")]
    [SerializeField] private SFXClip throwHookSFX;
    [SerializeField] private SFXClip disableHookSFX;
    #endregion


    #region References
    private new Rigidbody2D rigidbody;
    private HookThrower hookThrower;
    private OneShotSFXPlayer sfxPlayer;
    private RepeatAtFrequencySFXPlayer retractPlayer;
    #endregion

    bool isLocalPlayer;
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Water") && Mathf.Abs(rigidbody.velocity.y) > minVelocityForSplash)
        {
            if (isLocalPlayer)
            {
                sfxPlayer.RequestSFX(localSplashIn);
                underwaterSnapshot.TransitionTo(snapshotTransitionLength);
            }
            else
            {
                sfxPlayer.RequestSFX(remoteSplashIn);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            if (isLocalPlayer) normalGameplaySnapshot.TransitionTo(snapshotTransitionLength);
            if (Mathf.Abs(rigidbody.velocity.y) > minVelocityForSplash) sfxPlayer.RequestSFX(splashOut);
        }
    }

    #region Initialization
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
        isLocalPlayer = GetComponent<Photon.Pun.PhotonView>()?.IsMine ?? true;

        hookThrower.OnHookThrown += (Vector2 target) => sfxPlayer.RequestSFX(throwHookSFX);
        hookThrower.OnHookDisabled += OnHookDisabled;
        hookThrower.OnRetracted += (float amount) => retractAmountThisFrame = amount;
    }
    #endregion
}

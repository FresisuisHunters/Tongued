using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HookThrower), typeof(Rigidbody2D))]
public class ChamaleonAnimator : MonoBehaviour
{
    [Header("Animators")]
    [SerializeField] private Animator headAnimator;
    [SerializeField] private Animator bodyAnimator;

    [Header("Grounded Check")]
    [SerializeField] private LayerMask groundedCheckLayerMask;
    [SerializeField, Range(0, 90)] private float maxSlopeAngleForGrounded = 45;


    private new Rigidbody2D rigidbody;
    private ContactPoint2D[] contacts = new ContactPoint2D[0];
    private ContactFilter2D groundedCheckContactFilter;

    private static int IDLE_STATE = Animator.StringToHash("Idle");
    private static int GROUNDED_PROPERTY = Animator.StringToHash("IsGrounded");
    private static int THROW_STATE = Animator.StringToHash("Throw");
    private static int HANG_STATE = Animator.StringToHash("Hang");
    private static int POS_REACT_STATE = Animator.StringToHash("PosReact");
    private static int NEG_REACT_STATE = Animator.StringToHash("NegReact");


    //I think we can get waway with doing this in FixedUpdate.
    private void Update()
    {
        //TODO: Remove this from here once we're happy with the parameters. Just here for tweaking at runtime.
        groundedCheckContactFilter = new ContactFilter2D();
        groundedCheckContactFilter.ClearDepth();
        groundedCheckContactFilter.SetLayerMask(groundedCheckLayerMask);
        groundedCheckContactFilter.useTriggers = false;
        groundedCheckContactFilter.SetNormalAngle(-maxSlopeAngleForGrounded, maxSlopeAngleForGrounded);

        int groundedContactCount = rigidbody.GetContacts(groundedCheckContactFilter, contacts);
        bool isGrounded = groundedContactCount > 0;
        
        bodyAnimator.SetFloat(GROUNDED_PROPERTY, isGrounded ? 1f : 0f);

        //TODO: Align the body sprite with the ground normal.
    }

    private void OnHookThrown()
    {
        headAnimator.Play(THROW_STATE, 0, 0);
    }

    private void OnHookAttached()
    {
        headAnimator.Play(HANG_STATE, 0, 0);
        bodyAnimator.Play(HANG_STATE, 0, 0);
    }

    private void OnHookDisabled()
    {
        headAnimator.Play(IDLE_STATE, 0, 0);
        bodyAnimator.Play(IDLE_STATE, 0, 0);
    }

    private void DoPositiveReaction()
    {
        headAnimator.Play(POS_REACT_STATE, 1, 0);
    }

    private void DoNegativeReaction()
    {
        headAnimator.Play(NEG_REACT_STATE, 1, 0);
    }

    //TODO: Check for grounded animation.

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();

        groundedCheckContactFilter= new ContactFilter2D();
        groundedCheckContactFilter.ClearDepth();
        groundedCheckContactFilter.SetLayerMask(groundedCheckLayerMask);
        groundedCheckContactFilter.useTriggers = false;
        groundedCheckContactFilter.SetNormalAngle(-maxSlopeAngleForGrounded, maxSlopeAngleForGrounded);
    }

    private void Start()
    {
        HookThrower hookThrower = GetComponent<HookThrower>();
        hookThrower.OnHookThrown += OnHookThrown;
        hookThrower.OnHookAttached += OnHookAttached;
        hookThrower.OnHookDisabled += OnHookDisabled;

        //TODO: Subscribe to events that merit a positive or negative reaction
    }
}

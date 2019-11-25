using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 649
[RequireComponent(typeof(HookThrower), typeof(Rigidbody2D))]
public class ChamaleonAnimator : MonoBehaviour
{
    #region Constants
    private static int GROUNDED_PROPERTY = Animator.StringToHash("IsGrounded");
    private static int TONGUE_OUT_PROPERTY = Animator.StringToHash("TongueIsOut");
    private static int ATTACHED_PROPERTY = Animator.StringToHash("IsAttached");
    private static int POS_REACT_TRIGGER = Animator.StringToHash("PosReact");
    private static int NEG_REACT_TRIGGER = Animator.StringToHash("NegReact");
    #endregion

    #region Inspector
    [SerializeField]
    private Transform spritesParent;

    [Header("Animators")]
    [SerializeField] private Animator headAnimator;
    [SerializeField] private Animator bodyAnimator;

    [Header("Grounded Check")]
    [SerializeField, Range(0, 90)] private float maxSlopeAngleForGrounded = 45;
    #endregion

    #region References
    private new Rigidbody2D rigidbody;
    private HookThrower hookThrower;
    #endregion

    #region Private State
    private ContactPoint2D[] contacts = new ContactPoint2D[4];
    private float groundAngle;
    private float minGroundedDotProduct;

    bool stayUprightWhenAirborne;

    bool isGrounded;
    bool tongueIsOut;
    bool isAttached;
    #endregion

    private bool IsFacingRight
    {
        get
        {
            float currentRotation = transform.rotation.eulerAngles.z;
            return currentRotation < 90 || currentRotation > 270;
        }
    }

    private void FixedUpdate()
    {
        DoGroundedCheck();

        //if (isGrounded) FaceMovementDirection();

        if (tongueIsOut) RotateTowardsTongue();
        if (isGrounded) AlignWithGround();
    }

    private void Update()
    {
        if (stayUprightWhenAirborne || isGrounded) StayUpright();
    }

    #region Animation
    private void RotateTowardsTongue()
    {
        Vector2 u = hookThrower.SwingingHingePoint - (Vector2) spritesParent.position;
        Vector2 a = Vector2.right;

        float desiredAngle = Vector2.SignedAngle(a, u);
        float currentAngle = rigidbody.rotation;

        rigidbody.angularVelocity = (desiredAngle - currentAngle) / Time.deltaTime;
    }

    private void AlignWithGround()
    {
        transform.rotation = Quaternion.Euler(0, 0, IsFacingRight ? groundAngle : groundAngle + 180);
    }

    private void StayUpright()
    {
        float ySign = IsFacingRight ? 1 : -1;

        Vector3 scale = spritesParent.localScale;
        scale.y = ySign * Mathf.Abs(scale.y);
        spritesParent.localScale = scale;
    }
    #endregion

    #region Grounded Check
    private void DoGroundedCheck()
    {
        //Si el Rigidbody está dormido, sabemos que isGrounded no va a cambiar.
        if (rigidbody.IsAwake())
        {
            //Comprobamos si estamos tocando el suelo.
            int contactCount = rigidbody.GetContacts(contacts);
            Vector2 perfectGroundNormal = Vector2.up;   //TODO: Adapt this to account for cirular gravity map?
            Vector2 surfaceNormal;

            isGrounded = false;
            for (int i = 0; i < contactCount && !isGrounded; i++)
            {
                surfaceNormal = contacts[i].normal;
                if (Vector2.Dot(surfaceNormal, perfectGroundNormal) > minGroundedDotProduct)
                {
                    isGrounded = true;
                    groundAngle = Vector2.SignedAngle(perfectGroundNormal, surfaceNormal);
                }
            }

            rigidbody.freezeRotation = isGrounded;

            bodyAnimator.SetBool(GROUNDED_PROPERTY, isGrounded);
            headAnimator.SetBool(GROUNDED_PROPERTY, isGrounded);
        }
    }
    #endregion

    #region Event Responses
    private void OnHookThrown()
    {
        tongueIsOut = true;
        stayUprightWhenAirborne = true;
        
        headAnimator.SetBool(TONGUE_OUT_PROPERTY, true);
        headAnimator.SetBool(ATTACHED_PROPERTY, false);

        bodyAnimator.SetBool(ATTACHED_PROPERTY, false);
    }

    private void OnHookAttached()
    {
        tongueIsOut = true;
        stayUprightWhenAirborne = false;

        headAnimator.SetBool(TONGUE_OUT_PROPERTY, true);
        headAnimator.SetBool(ATTACHED_PROPERTY, true);

        bodyAnimator.SetBool(ATTACHED_PROPERTY, true);
    }

    private void OnHookDisabled()
    {
        tongueIsOut = false;
        stayUprightWhenAirborne = true;

        headAnimator.SetBool(TONGUE_OUT_PROPERTY, false);
        headAnimator.SetBool(ATTACHED_PROPERTY, false);
        
        bodyAnimator.SetBool(ATTACHED_PROPERTY, false);
    }

    private void DoPositiveReaction()
    {
        headAnimator.SetTrigger(POS_REACT_TRIGGER);
        bodyAnimator.SetTrigger(POS_REACT_TRIGGER);
    }

    private void DoNegativeReaction()
    {
        headAnimator.SetTrigger(NEG_REACT_TRIGGER);
        bodyAnimator.SetTrigger(NEG_REACT_TRIGGER);
    }
    #endregion

    #region Initialization
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        hookThrower = GetComponent<HookThrower>();
        OnValidate();
    }

    private void Start()
    {
        hookThrower.OnHookThrown += OnHookThrown;
        hookThrower.OnHookAttached += OnHookAttached;
        hookThrower.OnHookDisabled += OnHookDisabled;

        //TODO: Subscribe to events that merit a positive or negative reaction
    }

    private void OnValidate()
    {
        minGroundedDotProduct = Mathf.Cos(maxSlopeAngleForGrounded * Mathf.Deg2Rad);
    }
    #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 649
[RequireComponent(typeof(HookThrower), typeof(Rigidbody2D))]
public class ChamaleonAnimator : MonoBehaviour
{
    [Header("Animators")]
    [SerializeField] private Animator headAnimator;
    [SerializeField] private Animator bodyAnimator;
    
    [Header("Grounded Check")]
    [SerializeField, Range(0, 90)] private float maxSlopeAngleForGrounded = 45;

    private new Rigidbody2D rigidbody;
    private ContactPoint2D[] contacts = new ContactPoint2D[4];
    private float minGroundedDotProduct;
    bool isGrounded;

    private static int GROUNDED_PROPERTY = Animator.StringToHash("IsGrounded");
    private static int TONGUE_OUT_PROPERTY = Animator.StringToHash("TongueIsOut");
    private static int ATTACHED_PROPERTY = Animator.StringToHash("IsAttached");
    private static int POS_REACT_TRIGGER = Animator.StringToHash("PosReact");
    private static int NEG_REACT_TRIGGER = Animator.StringToHash("NegReact");

    private void Update()
    {
        bodyAnimator.SetBool(GROUNDED_PROPERTY, isGrounded);
        
    }

    #region Grounded Check
    private void FixedUpdate()
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
                if (Vector2.Dot(surfaceNormal, perfectGroundNormal) > minGroundedDotProduct) isGrounded = true;
            }
        }
    }
    #endregion

    #region Event Responses
    private void OnHookThrown()
    {
        headAnimator.SetBool(TONGUE_OUT_PROPERTY, true);
        headAnimator.SetBool(ATTACHED_PROPERTY, false);

        bodyAnimator.SetBool(ATTACHED_PROPERTY, false);
    }

    private void OnHookAttached()
    {
        headAnimator.SetBool(TONGUE_OUT_PROPERTY, true);
        headAnimator.SetBool(ATTACHED_PROPERTY, true);

        bodyAnimator.SetBool(ATTACHED_PROPERTY, true);
    }

    private void OnHookDisabled()
    {
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
        OnValidate();
    }

    private void Start()
    {
        HookThrower hookThrower = GetComponent<HookThrower>();
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

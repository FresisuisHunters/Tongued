﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HookThrower), typeof(Rigidbody2D))]
public class ChamaleonAnimator : MonoBehaviour
{
    [Header("Animators")]
    [SerializeField] private Animator headAnimator;
    [SerializeField] private Animator bodyAnimator;

    [Header("Grounded Check")]
    [SerializeField, Range(0, 90)] private float maxSlopeAngleForGrounded = 45;

    private new Rigidbody2D rigidbody;
    private ContactPoint2D[] contacts = new ContactPoint2D[1];
    private float minGroundedDotProduct;

    private static int IDLE_STATE = Animator.StringToHash("Idle");
    private static int GROUNDED_PROPERTY = Animator.StringToHash("IsGrounded");
    private static int THROW_STATE = Animator.StringToHash("Throw");
    private static int HANG_STATE = Animator.StringToHash("Hang");
    private static int POS_REACT_STATE = Animator.StringToHash("PosReact");
    private static int NEG_REACT_STATE = Animator.StringToHash("NegReact");

    private void OnCollisionStay2D(Collision2D collision)
    {
        int contactCount = collision.GetContacts(contacts);
        Vector2 perfectGroundNormal = Vector2.up;   //TODO: Adapt this to account for cirular gravity map?
        Vector2 surfaceNormal;
        bool isGrounded = false;

        for (int i = 0; i < contactCount && !isGrounded; i++)
        {
            surfaceNormal = contacts[i].normal;
            isGrounded = Vector2.Dot(surfaceNormal, perfectGroundNormal) > minGroundedDotProduct;
        }

        bodyAnimator.SetFloat(GROUNDED_PROPERTY, isGrounded ? 1f : 0f);
    }

    #region Event Responses
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
    #endregion

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
}

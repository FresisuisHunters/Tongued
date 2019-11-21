using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChamaleonAnimator : MonoBehaviour
{
    [SerializeField] private Animator headAnimator;
    [SerializeField] private Animator bodyAnimator;

    private static int IDLE_STATE = Animator.StringToHash("Idle");
    private static int GROUNDED_STATE = Animator.StringToHash("Grounded");
    private static int THROW_STATE = Animator.StringToHash("Throw");
    private static int HANG_STATE = Animator.StringToHash("Hang");
    private static int FALL_STATE = Animator.StringToHash("Fall");
    private static int POS_REACT_STATE = Animator.StringToHash("PosReact");
    private static int NEG_REACT_STATE = Animator.StringToHash("NegReact");

    private void Start()
    {
        headAnimator.Play(HANG_STATE, 0, 0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            headAnimator.Play(POS_REACT_STATE, 1, 0);
        }
    }
}

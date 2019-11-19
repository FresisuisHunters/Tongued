using UnityEngine;

[RequireComponent (typeof (HookThrower))]
public class TouchHookInput : MonoBehaviour {

    private HookThrower hookThrower;
    private Vector3 pressedPosition;
    private bool isPressingScreen;
    private int fingerId;

    private void Awake () {
        hookThrower = GetComponent<HookThrower> ();
        fingerId = -1;
    }

    private void Update () {
        bool fingerLifted = true;
        foreach (Touch touchInput in Input.touches) {
            int touchFingerId = touchInput.fingerId;
            if (fingerId == -1) {
                OnScreenPressed (touchInput);
            }

            if (touchFingerId != fingerId) {
                continue;
            }

            fingerLifted = false;
        }

        if (fingerLifted) {
            OnFingerLifted ();
        } else {
            HandleInput ();
        }
    }

    private void OnScreenPressed (Touch touchInput) {
        isPressingScreen = true;
        fingerId = touchInput.fingerId;
        pressedPosition = Camera.main.ScreenToWorldPoint (touchInput.position);
    }

    private void OnFingerLifted () {
        isPressingScreen = false;
        fingerId = -1;

        hookThrower.LetGo ();
    }

    private void HandleInput () {
        if (!hookThrower.HookIsOut) {
            hookThrower.ThrowHook (pressedPosition);
        }

        if (hookThrower.HookIsOut) {
            hookThrower.Retract (Time.deltaTime);
        }
    }

}
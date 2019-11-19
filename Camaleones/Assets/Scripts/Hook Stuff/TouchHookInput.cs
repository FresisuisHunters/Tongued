using UnityEngine;

[RequireComponent (typeof (HookThrower))]
public class TouchHookInput : MonoBehaviour {

    private const int NO_FINGER = -1;

    private HookThrower hookThrower;
    private Vector3 pressedPosition;
    private bool isPressingScreen;
    private int fingerId;

    private void Awake () {
        hookThrower = GetComponent<HookThrower> ();
        fingerId = NO_FINGER;
    }

    private void Update () {
        if (Input.touchCount == 0) {
            return;
        }

        bool fingerLifted = !HandleScreenTouches();
        if (fingerLifted) {
            OnFingerLifted ();
        } else if (hookThrower.HookIsOut) {
            hookThrower.Retract (Time.deltaTime);
        }
    }

    private bool HandleScreenTouches() {
        // TODO: Es un poco confuso el fingerLifted
        bool fingerLifted = true;
        foreach (Touch touchInput in Input.touches) {
            if (fingerId == NO_FINGER) {
                OnScreenPressed (touchInput);
            }

            if (touchInput.fingerId != fingerId) {
                continue;
            }

            fingerLifted = false;
        }

        return !fingerLifted;
    }

    private void OnScreenPressed (Touch touchInput) {
        isPressingScreen = true;
        fingerId = touchInput.fingerId;
        pressedPosition = Camera.main.ScreenToWorldPoint (touchInput.position);

        if (!hookThrower.HookIsOut) {
            hookThrower.ThrowHook (pressedPosition);
        }
    }

    private void OnFingerLifted () {
        isPressingScreen = false;
        fingerId = NO_FINGER;

        hookThrower.LetGo ();
    }

}
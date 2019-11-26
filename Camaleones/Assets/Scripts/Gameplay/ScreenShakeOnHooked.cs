using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShakeOnHooked : Cinemachine.CinemachineImpulseSource, IOnHookedListener
{
    public void OnHooked(Vector2 pullDirection) => base.GenerateImpulse(pullDirection);
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShakeOnHooked : Cinemachine.CinemachineImpulseSource, IOnHookedListener
{
    void IOnHookedListener.OnHooked(Vector2 pullDirection, Hook hook) => base.GenerateImpulse(pullDirection);
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hairibar.Audio.SFX;

[RequireComponent(typeof(SFXPlayer), typeof(HookThrower), typeof(Rigidbody2D))]
public class ChamaleonSFX : MonoBehaviour
{
    [SerializeField] private SFXClip throwHookSFX;
    [SerializeField] private SFXClip disableHookSFX;
    [SerializeField] private SFXClip flyingSFX;
}

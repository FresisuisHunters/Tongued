using UnityEngine;
using Hairibar.Audio.SFX;

#pragma warning disable 649
[RequireComponent(typeof(OneShotSFXPlayer))]
public class SFXOnStart : MonoBehaviour
{
    [SerializeField]
    private SFXClip sfx;

    void Start()
    {
        GetComponent<OneShotSFXPlayer>().RequestSFX(sfx);
        Destroy(this);
    }
}

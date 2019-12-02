using UnityEngine;
using Hairibar.Audio.SFX;

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

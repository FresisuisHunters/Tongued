using UnityEngine;
using Hairibar.Audio.SFX;

[RequireComponent(typeof(SFXPlayer))]
public class SFXOnStart : MonoBehaviour
{
    [SerializeField]
    private SFXClip sfx;

    void Start()
    {
        GetComponent<SFXPlayer>().RequestSFX(sfx);
        Destroy(this);
    }
}

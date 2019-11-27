using UnityEngine;

namespace Hairibar.Audio.SFX
{
    [RequireComponent(typeof(SFXPlayer))]
    public class DestroyAfterSFX : MonoBehaviour
    {
        [SerializeField]
        private SFXClip sfxClip;

        private void Start()
        {
            GetComponent<SFXPlayer>().RequestSFX(sfxClip, DestroySelf);
        }

        private void DestroySelf()
        {
            Destroy(gameObject);
        }
    }
}



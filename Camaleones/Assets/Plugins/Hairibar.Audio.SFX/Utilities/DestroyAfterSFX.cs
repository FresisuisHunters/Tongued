using UnityEngine;

namespace Hairibar.Audio.SFX
{
    [RequireComponent(typeof(OneShotSFXPlayer))]
    public class DestroyAfterSFX : MonoBehaviour
    {
        [SerializeField]
        private SFXClip sfxClip;

        private void Start()
        {
            GetComponent<OneShotSFXPlayer>().RequestSFX(sfxClip, DestroySelf);
        }

        private void DestroySelf()
        {
            Destroy(gameObject);
        }
    }
}



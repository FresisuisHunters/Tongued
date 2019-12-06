using UnityEngine;
using Photon.Pun;

public class DisableSpatialAudioIfLocalPlayer : MonoBehaviour
{
    private void Start()
    {
        PhotonView photonView = GetComponent<PhotonView>();
        bool isLocalPlayer = photonView?.IsMine ?? true;

        if (isLocalPlayer)
        {
            foreach (AudioSource audioSource in GetComponentsInChildren<AudioSource>(true))
            {
                audioSource.spatialBlend = 0;
            }
        }

        Destroy(this);
    }
}

using Photon.Pun;
using UnityEngine;

#pragma warning disable 649
[RequireComponent(typeof(PhotonView))]
public class RemoveComponentsIfRemotePlayer : MonoBehaviour
{
    [SerializeField] private MonoBehaviour[] componentsToRemove;

    private void Awake()
    {
        if (!GetComponent<PhotonView>().IsMine)
        {
            for (int i = 0; i < componentsToRemove.Length; i++)
            {
                if (componentsToRemove[i] != null)
                {
                    Destroy(componentsToRemove[i]);
                }
            }
        }

        Destroy(this);
    }
}

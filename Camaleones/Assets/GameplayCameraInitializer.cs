using UnityEngine;
using Photon.Pun;

/// <summary>
/// Inicializa el Gameplay Camera Setup automáticamente, buscando al jugador local y las bounds del escenario.
/// </summary>
public class GameplayCameraInitializer : MonoBehaviour
{
    void Start()
    {
        Transform playerTransform = null;

        PhotonView[] photonView = FindObjectsOfType<PhotonView>();
        if (photonView.Length == 0)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }
        else
        {
            for (int i = 0; i < photonView.Length; i++)
            {
                if (photonView[i].IsMine)
                {
                    playerTransform = photonView[i].transform;
                    break;
                }
            }
        }

        GetComponentInChildren<Cinemachine.CinemachineVirtualCamera>().Follow = playerTransform;

        GetComponentInChildren<Cinemachine.CinemachineConfiner>().m_BoundingShape2D = GameObject.FindGameObjectWithTag("CameraBounds").GetComponent<CompositeCollider2D>();

        Destroy(this);
    }    
}

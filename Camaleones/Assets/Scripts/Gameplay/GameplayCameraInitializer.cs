using UnityEngine;
using Photon.Pun;

/// <summary>
/// Inicializa el Gameplay Camera Setup automáticamente, buscando al jugador local y las bounds del escenario.
/// </summary>
public class GameplayCameraInitializer : MonoBehaviour
{
    void Start()
    { 
        FindPlayer();
        FindBoundingShape();
        AssignCameraAtInputReceiver();

        Destroy(this);
    }    

    private void FindPlayer()
    {
        Transform playerTransform = null;

        PhotonView[] photonViews = FindObjectsOfType<PhotonView>();
        if (photonViews.Length == 0)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }
        else
        {
            for (int i = 0; i < photonViews.Length; i++)
            {
                if (photonViews[i].IsMine && photonViews[i].CompareTag("Player"))
                {
                    playerTransform = photonViews[i].transform;
                    break;
                }
            }
        }

        GetComponentInChildren<Cinemachine.CinemachineVirtualCamera>().Follow = playerTransform;
    }

    private void FindBoundingShape()
    {
        GetComponentInChildren<Cinemachine.CinemachineConfiner>().m_BoundingShape2D = GameObject.FindGameObjectWithTag("CameraBounds").GetComponent<CompositeCollider2D>();
    }

    private void AssignCameraAtInputReceiver()
    {
        FindObjectOfType<InputEventReceiver>().GetComponent<Canvas>().worldCamera = GetComponentInChildren<Camera>();
    }
    
}

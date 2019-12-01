using UnityEngine;
using Photon.Pun;

/// <summary>
/// Inicializa el Gameplay Camera Setup automáticamente, buscando al jugador local y las bounds del escenario.
/// </summary>
public class GameplayCameraInitializer : MonoBehaviour
{
    void Start()
    { 
        FindBoundingShape();
        AssignCameraAtInputReceiver();
    }

    private void Update()
    {
        if (FindPlayer()) Destroy(this);
    }

    private bool FindPlayer()
    {
        GameObject[] potentialPlayers = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < potentialPlayers.Length; i++)
        {
            PhotonView photonView = potentialPlayers[i].GetComponent<PhotonView>();
            if (photonView)
            {
                if (photonView.IsMine)
                {
                    GetComponentInChildren<Cinemachine.CinemachineVirtualCamera>().Follow = photonView.transform;
                    return true;
                }
            }
            else
            {
                GetComponentInChildren<Cinemachine.CinemachineVirtualCamera>().Follow = potentialPlayers[i].transform;
                return true;
            }
        }

        return false;
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

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
        if (InitializationUtilities.FindLocalPlayer(out Transform player))
        {
            GetComponentInChildren<Cinemachine.CinemachineVirtualCamera>().Follow = player;
            return true;
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

using UnityEngine;
using Photon.Pun;

public static class InitializationUtilities
{
    public static bool FindLocalPlayer(out GameObject player)
    {
        GameObject[] potentialPlayers = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < potentialPlayers.Length; i++)
        {
            PhotonView photonView = potentialPlayers[i].GetComponent<PhotonView>();
            if (photonView)
            {
                if (photonView.IsMine)
                {
                    player = photonView.gameObject;
                    return true;
                }
            }
            else
            {
                player = potentialPlayers[i];
                return true;
            }
        }

        player = null;
        return false;
    }

    public static bool FindLocalPlayer<T>(out T player) where T : Component
    {
        if (FindLocalPlayer(out GameObject playerGO))
        {
            player = playerGO.GetComponent<T>();
            return true;
        }

        player = null;
        return false;
    }
}

using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnlinePlayer : MonoBehaviourPunCallbacks {

    #region Private Methods

    [PunRPC]
    private void DestroyPlayer (GameObject player) {
        GameObject.Destroy (player);
    }

    private void DestroySelfAndReturnToMenu () {
        photonView.RPC ("DestroyPlayer", RpcTarget.Others, new object[] { gameObject });
        SceneManager.LoadScene ("Lobby_Scene");
    }

    #endregion

    #region Photon Callbacks

    public override void OnDisconnected (DisconnectCause cause) {
        Debug.Log ("OnDisconnected");
        Debug.LogError (cause);

        DestroySelfAndReturnToMenu ();
    }

    public override void OnPlayerLeftRoom (Player otherPlayer) {
        Debug.Log ("OnPlayerLeftRoom");
        Debug.Log (otherPlayer);

        string playerWhoLeftTheRoom = otherPlayer.NickName;
        GameObject[] allPlayers = GameObject.FindGameObjectsWithTag ("Player");
        foreach (GameObject player in allPlayers) {
            if (player.name.Equals (playerWhoLeftTheRoom)) {
                DestroyPlayer (player);
                break;
            }
        }
    }

    public override void OnLeftRoom () {
        Debug.Log ("OnLeftRoom");

        DestroySelfAndReturnToMenu ();
    }

    #endregion
}
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnlinePlayer : MonoBehaviourPunCallbacks {

    [System.NonSerialized] public OnlinePlayer localInstance;

    private OnlineLogging onlineLogging;

    #region Unity Callbacks

    protected void Awake () {
        if (photonView.IsMine) {
            localInstance = this;
            onlineLogging = new OnlineLogging (PhotonNetwork.LocalPlayer.NickName);
        }
    }

    #endregion

    #region Private Methods

    private GameObject FindPlayerWithName (string username) {
        GameObject[] allPlayers = GameObject.FindGameObjectsWithTag ("Player");
        foreach (GameObject player in allPlayers) {
            if (player.name.Equals (username)) {
                return player;
            }
        }

        throw new System.Exception ("No player found with username " + username);
    }

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
        onlineLogging.Write ("OnDisconnected");
        onlineLogging.Write (cause.ToString ());

        DestroySelfAndReturnToMenu ();
    }

    public override void OnPlayerLeftRoom (Player otherPlayer) {
        onlineLogging.Write ("OnPlayerLeftRoom");
        onlineLogging.Write (otherPlayer.ToString ());

        GameObject player = FindPlayerWithName (otherPlayer.NickName);
        DestroyPlayer (player);
    }

    public override void OnLeftRoom () {
        onlineLogging.Write ("OnLeftRoom");

        DestroySelfAndReturnToMenu ();
    }

    #endregion
}
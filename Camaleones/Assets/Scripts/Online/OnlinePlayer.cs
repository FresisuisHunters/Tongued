using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class OnlinePlayer : MonoBehaviourPunCallbacks {

    [System.NonSerialized] public OnlinePlayer localInstance;
    public TextMeshPro playerNameOnScreen;

    private Vector3 positionOffset;

    #region Unity Callbacks

    protected void Awake () {
        playerNameOnScreen.text = photonView.Owner.NickName;
        positionOffset = transform.position - playerNameOnScreen.rectTransform.position;
        if (photonView.IsMine) {
            playerNameOnScreen.color = Color.yellow;
            localInstance = this;
        }
    }

    protected void LateUpdate() {
        // Para que no se mueva/rote el texto junto con el camaleon
        playerNameOnScreen.rectTransform.rotation = Quaternion.identity;
        playerNameOnScreen.rectTransform.position = transform.position - positionOffset;
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

    private void DestroyPlayer (GameObject player) {
        PhotonNetwork.Destroy(player);
    }

    private void DestroySelfAndReturnToMenu () {
        DestroyPlayer(gameObject);
        SceneManager.LoadScene ("Lobby_Scene");
    }

    #endregion

    #region Photon Callbacks

    public override void OnDisconnected (DisconnectCause cause) {
        string log = string.Format("OnDisconnected. Cause: {0}", cause);
        OnlineLogging.Instance.Write(log);

        DestroySelfAndReturnToMenu ();
    }

    public override void OnPlayerLeftRoom (Player otherPlayer) {
        string log= string.Format("OnPlayerLeftRoom: {0}", otherPlayer.ToStringFull());
        OnlineLogging.Instance.Write(log);
    }

    public override void OnLeftRoom () {
        OnlineLogging.Instance.Write("OnLeftRoom");

        DestroySelfAndReturnToMenu ();
    }

    #endregion
}
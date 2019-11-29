using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class OnlinePlayer : MonoBehaviourPunCallbacks {

    [System.NonSerialized] public OnlinePlayer localInstance;
    private TextMeshProUGUI nameOnScreen;
    private Canvas playerNamesCanvas;

    #region Unity Callbacks

    protected void Awake () {
        playerNamesCanvas = GameObject.FindGameObjectWithTag("NicknameCanvas").GetComponent<Canvas>();
        if (photonView.IsMine) {
            localInstance = this;
        }
    }

    protected void Update() {
        if (!photonView.IsMine) {
            return;
        }

        Vector2 namePosition = new Vector2
        (
            transform.position.x,
            transform.position.y
        );
        
        Vector2 viewPos = Camera.main.WorldToViewportPoint(transform.position);
        nameOnScreen.rectTransform.anchoredPosition = new Vector2
        (
            viewPos.x * playerNamesCanvas.pixelRect.size.x,
            viewPos.y * playerNamesCanvas.pixelRect.size.y
        );
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

    #region Properties

    public TextMeshProUGUI NicknameOnCanvas {
        get => nameOnScreen;
        set => nameOnScreen = value;
    }

    #endregion

}
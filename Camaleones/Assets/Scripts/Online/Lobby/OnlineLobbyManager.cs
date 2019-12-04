using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

#pragma warning disable 649
public class OnlineLobbyManager : MonoBehaviourPunCallbacks {

    #region Constant Fields
    private const string CONNECTION_STATUS_MESSAGE = "Connection status:\n";
    #endregion

    [SerializeField] private TextMeshProUGUI connectionStatusText;
    
    #region Unity Callbacks
    private void Awake () {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnEnable() {
        base.OnEnable();

        if (PhotonNetwork.IsConnectedAndReady) {
            SwitchToLobbyMainMenu();
        }
    }

    private void Update () {
        connectionStatusText.text = CONNECTION_STATUS_MESSAGE + PhotonNetwork.NetworkClientState;
    }

    
    #endregion

    #region Photon Callbacks
    public override void OnDisconnected (DisconnectCause cause) {
        string log = string.Format("Player disconnected. Cause: {0}", cause);
        OnlineLogging.Instance.Write(log);

        GetComponent<MenuScreenManager>().SetActiveMenuScreen<AskUsernameScreen>();
    }

    public override void OnLeftRoom () {
        GetComponent<MenuScreenManager>().SetActiveMenuScreen<LobbyScreen>();
    }
    #endregion
}
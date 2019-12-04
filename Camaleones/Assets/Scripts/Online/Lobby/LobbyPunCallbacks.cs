using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(LobbyScreen))]
public class LobbyPunCallbacks : MonoBehaviourPunCallbacks
{
    public override void OnJoinedLobby()
    {
        GetComponent<LobbyScreen>().JoinOrCreateRoom();
    }

    public override void OnJoinedRoom()
    {
        GetComponent<LobbyScreen>().GoToRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        GetComponent<LobbyScreen>().OnJoinRandomFailed(returnCode, message);
    }

    public override void OnCreatedRoom()
    {
        OnlineLobbyManager.Instance.SwitchToRoomPanel();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        string log = string.Format("Error creating room\nReturn code: {0}\nError message: {1}", returnCode, message);
        OnlineLogging.Instance.Write(log);

        //MakeUIInteractable();
    }
}

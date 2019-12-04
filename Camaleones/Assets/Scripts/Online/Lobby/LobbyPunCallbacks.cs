using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(LobbyScreen))]
public class LobbyPunCallbacks : MonoBehaviourPunCallbacks
{
    public override void OnJoinedLobby()
    {
        GetComponent<LobbyScreen>().JoinOrCreatePublicRoom();
    }

    public override void OnJoinedRoom()
    {
        GetComponent<LobbyScreen>().GoToRoom();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        GetComponent<LobbyScreen>().OnJoinRoomFailed(returnCode, message);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        GetComponent<LobbyScreen>().OnJoinRandomFailed(returnCode, message);
    }

    public override void OnCreatedRoom()
    {
        GetComponent<LobbyScreen>().GoToRoom();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        GetComponent<LobbyScreen>().OnCreateRoomFailed(returnCode, message);
    }
}

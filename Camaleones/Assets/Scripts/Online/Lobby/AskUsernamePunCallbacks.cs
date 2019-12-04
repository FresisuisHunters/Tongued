using Photon.Pun;
using UnityEngine;

#pragma warning disable 649
[RequireComponent(typeof(AskUsernameScreen))]
public class AskUsernamePunCallbacks : MonoBehaviourPunCallbacks
{
    public override void OnConnectedToMaster()
    {
        GetComponent<AskUsernameScreen>().GoToLobbyScreen();
        Debug.Log("connected to master");
    }
}


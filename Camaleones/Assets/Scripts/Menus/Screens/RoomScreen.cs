using UnityEngine;
using Photon.Pun;
using System;

public class RoomScreen : AMenuScreen
{
    [SerializeField] private GameObject roomPanel;

    protected override void OnOpen(Type previousScreen)
    {
        roomPanel.SetActive(true);
        roomPanel.GetComponent<RoomPanel>().Enable();
    }

    protected override void OnClose(Type nextScreen)
    {
        roomPanel.SetActive(false);
    }

    public override void GoBack()
    {
        PhotonNetwork.LeaveRoom();
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

#pragma warning disable 649
public class RoomPlayerEntry : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private Button playerReadyButton;
    [SerializeField] private Button playerWaitingButton;

    [SerializeField] private Material localPlayerNameMaterial;
    [SerializeField] private Material remotePlayerNameMaterial;

    public string PlayerName
    {
        get => playerNameText.text;
        set
        {
            playerNameText.text = value;

            bool isLocalPlayer = PhotonNetwork.LocalPlayer.NickName.Equals(value);
            playerReadyButton.enabled = isLocalPlayer;
            playerWaitingButton.enabled = isLocalPlayer;
        }
    }

    public bool IsReady
    {
        set
        {
            playerReadyButton.gameObject.SetActive(!value);
            playerWaitingButton.gameObject.SetActive(value);
        }
    }

    public bool IsLocalPlayer { set => playerNameText.fontMaterial = value ? localPlayerNameMaterial : remotePlayerNameMaterial; }
        


    public void OnReadyButtonPressed(bool isReady)
    {
        GetComponentInParent<RoomScreen>().SetLocalPlayerReady(isReady);

        IsReady = isReady;
    }

}
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Linq;

#pragma warning disable 649
public class LobbyScreen : AMenuScreen
{
    [SerializeField] private TMPro.TMP_InputField roomNameInput;
    [SerializeField] private Button joinPublicRoomButton;
    [SerializeField] private Button cancelPublicSearchButton;
    [SerializeField] private Button backButton;
    [SerializeField] private TMPro.TextMeshProUGUI messageField;

    private byte currentAtempts;

    #region Lifetime
    protected override void OnOpen(System.Type previousScreen)
    {
        currentAtempts = 0;
    }

    public override void GoBack()
    {
        PhotonNetwork.Disconnect();
        MenuManager.SetActiveMenuScreen<AskUsernameScreen>();
    }
    #endregion

    #region Public rooms
    public void JoinPublicRoom()
    {
        TypedLobby lobby = ServerConstants.GAME_MODE_1_LOBBY;
        PhotonNetwork.JoinLobby(lobby);

        joinPublicRoomButton.gameObject.SetActive(false);
        cancelPublicSearchButton.gameObject.SetActive(true);

        SetInteractable(false, cancelPublicSearchButton, backButton);
    }

    public void CancelPublicSearch()
    {
        PhotonNetwork.LeaveLobby();
        CancelInvoke();

        joinPublicRoomButton.gameObject.SetActive(true);
        cancelPublicSearchButton.gameObject.SetActive(false);

        SetInteractable(true);
    }
    #endregion

    #region Private rooms
    public void CreatePrivateRoom()
    {
        string roomName = roomNameInput.text;
        if (string.IsNullOrEmpty(roomName) || string.IsNullOrWhiteSpace(roomName))
        {
            return;
        }

        SetInteractable(false);

        byte roomSize = 4;
        string gameModeString = ServerConstants.GAME_MODE_1;

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = roomSize;
        roomOptions.IsVisible = false;
        roomOptions.CustomRoomProperties = new Hashtable();
        roomOptions.CustomRoomProperties.Add(ServerConstants.GAME_MODE_ROOM_KEY, gameModeString);

        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    public void JoinPrivateRoom()
    {
        string roomName = roomNameInput.text;
        if (string.IsNullOrEmpty(roomName) || string.IsNullOrWhiteSpace(roomName))
        {
            return;
        }

        SetInteractable(false);
        PhotonNetwork.JoinRoom(roomName);
    }
    #endregion

    public void GoToRoom()
    {
        MenuManager.SetActiveMenuScreen<RoomScreen>();
        OnlineLogging.Instance.Write(PhotonNetwork.CurrentRoom.ToStringFull());
    }

    #region Photon Callbacks
    public void JoinOrCreatePublicRoom()
    {
        Hashtable roomProperties = new Hashtable();
        roomProperties.Add(ServerConstants.GAME_MODE_ROOM_KEY, ServerConstants.GAME_MODE_1);

        if (currentAtempts < ServerConstants.JOIN_RANDOM_ROOM_TRIES)
        {
            PhotonNetwork.JoinRandomRoom(roomProperties, 0, MatchmakingMode.FillRoom, TypedLobby.Default, "");
        }
        else
        {
            string roomName = RandomString(10);
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 4;
            roomOptions.CustomRoomProperties = roomProperties;
            roomOptions.CustomRoomPropertiesForLobby = new string[] { ServerConstants.GAME_MODE_1 };

            PhotonNetwork.CreateRoom(roomName, roomOptions, TypedLobby.Default);
        }
    }

    public void OnJoinRandomFailed(short returnCode, string message)
    {
        currentAtempts++;
        string log = string.Format("Failed to join public room. Try number {0}. Return code: {1}. Message: {2}", currentAtempts, returnCode, message);
        OnlineLogging.Instance.Write(log);
        
        Invoke("JoinPublicRoom", ServerConstants.JOIN_RANDOM_RETRY_TIME);
    }

    public void OnJoinRoomFailed(short returnCode, string message)
    {
        string log = string.Format("Couldn't join room. Return code: {0}. Error message: {1}", returnCode, message);
        OnlineLogging.Instance.Write(log);

        SetInteractable(true);
        messageField.text = message;
    }

    public void OnCreateRoomFailed(short returnCode, string message)
    {
        string log = string.Format("Error creating room\nReturn code: {0}\nError message: {1}", returnCode, message);
        OnlineLogging.Instance.Write(log);

        SetInteractable(true);
    }
    #endregion

    private void Awake()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private static string RandomString(int length)
    {
        System.Random random = new System.Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}

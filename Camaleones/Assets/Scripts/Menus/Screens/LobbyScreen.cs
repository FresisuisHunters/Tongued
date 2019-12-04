using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Linq;

public class LobbyScreen : AMenuScreen
{
    [SerializeField] private TMPro.TMP_InputField roomNameInput;

    private byte currentAtempts;


    protected override void OnOpen(System.Type previousScreen)
    {
        currentAtempts = 0;
    }


    public void JoinPublicRoom()
    {
        TypedLobby lobby = ServerConstants.GAME_MODE_1_LOBBY;
        PhotonNetwork.JoinLobby(lobby);
    }

    public void CreatePrivateRoom()
    {
        string roomName = roomNameInput.text;
        if (string.IsNullOrEmpty(roomName) || string.IsNullOrWhiteSpace(roomName))
        {
            return;
        }

        //MakeUINonInteractable();

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
        //OnlineLobbyManager.Instance.SwitchToJoinPrivateRoomPanel();
    }

    public void QuitLobby()
    {
        PhotonNetwork.Disconnect();
    }

    public void GoToRoom()
    {
        //OnlineLobbyManager.Instance.SwitchToRoomPanel();
        OnlineLogging.Instance.Write(PhotonNetwork.CurrentRoom.ToStringFull());
    }

    #region Photon Callbacks
    public void JoinOrCreateRoom()
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

        Invoke("OnJoinPublicGameButtonClicked", ServerConstants.JOIN_RANDOM_RETRY_TIME);
    }
    #endregion






    private static string RandomString(int length)
    {
        System.Random random = new System.Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}

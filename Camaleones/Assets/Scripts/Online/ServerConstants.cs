using Photon.Realtime;
using System.Collections.Generic;

public class ServerConstants {

    public const string GAME_VERSION = "b0.1";
    public const string ONLINE_LEVEL = "sce_tAnimationDev";

    #region Game Mode Constants

    public const string GAME_MODE_1 = "Mode 1";
    public const string GAME_MODE_2 = "Mode 2";
    public const string GAME_MODE_3 = "Mode 3";
    public static readonly string[] GAME_MODES = new string[] { GAME_MODE_1, GAME_MODE_2, GAME_MODE_3 };

    #endregion

    #region Lobbies

    public static readonly TypedLobby GAME_MODE_1_LOBBY = new TypedLobby(GAME_MODE_1, LobbyType.SqlLobby);
    public static readonly TypedLobby GAME_MODE_2_LOBBY = new TypedLobby(GAME_MODE_2, LobbyType.SqlLobby);
    public static readonly TypedLobby GAME_MODE_3_LOBBY = new TypedLobby(GAME_MODE_3, LobbyType.SqlLobby);
    
    // TODO: Tal vez no hardcodearlos
    public static readonly Dictionary<string, TypedLobby> GAME_MODES_LOBBIES = new Dictionary<string, TypedLobby> {
        { GAME_MODE_1, GAME_MODE_1_LOBBY },
        { GAME_MODE_2, GAME_MODE_2_LOBBY },
        { GAME_MODE_3, GAME_MODE_3_LOBBY }
    };

    #endregion

    #region Room Creation Constants

    public static readonly string[] ROOM_SIZES = new string[] { "2", "3", "4", "5", "6" };
    public const string GAME_MODE_ROOM_KEY =  "GameMode";
    public const byte PUBLIC_ROOM_SIZE = 4;
    public const byte JOIN_RANDOM_ROOM_TRIES = 5;
    public const float JOIN_RANDOM_RETRY_TIME = 2.0f;

    #endregion
    
    #region Room Constants

    public const string PLAYERS_READY_KEY = "PLAYERS_READY";

    #endregion

}
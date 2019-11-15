public class ServerConstants {

    public const string GAME_VERSION = "b0.1";
    public const string ONLINE_LEVEL = "Online_Level";

    #region Game Mode Constants

    public const string GAME_MODE_1 = "Mode 1";
    public const string GAME_MODE_2 = "Mode 2";
    public const string GAME_MODE_3 = "Mode 3";
    public static readonly string[] GAME_MODES = new string[] { GAME_MODE_1, GAME_MODE_2, GAME_MODE_3 };

    #endregion

    #region Room Creation Constants

    public static readonly string[] ROOM_SIZES = new string[] { "2", "3", "4", "5", "6" };
    public const string GAME_MODE_ROOM_KEY =  "GameMode";
    public const byte JOIN_RANDOM_ROOM_TRIES = 5;
    public const float JOIN_RANDOM_RETRY_TIME = 2.0f;

    #endregion
    
}
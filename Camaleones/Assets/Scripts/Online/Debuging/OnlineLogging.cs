using System.IO;
using UnityEngine;
using Photon.Pun;

public class OnlineLogging {

    private static OnlineLogging instance;
    private string player;
    private string path;
    private StreamWriter streamWriter;
    private bool closed;

    private OnlineLogging (string player) {
        this.player = player;
        this.closed = Application.isEditor; // To create log files only in builds
        if (this.closed) {
            return;
        }

        this.path = string.Format ("./{0}.txt", player);
        this.streamWriter = new StreamWriter (this.path, false);

        Application.quitting += Close;
    }

    public void Write (string text) {
        string outputText = string.Format ("{0}: {1}\n", player, text);
        Debug.Log(outputText);

        if (!closed) {
            streamWriter.Write (outputText);
        }
    }

    public void Close () {
        if (closed) {
            return;
        }

        streamWriter.Close ();
        closed = true;
    }

    public static OnlineLogging Instance {
        get {
            if (instance == null) {
                string username = PhotonNetwork.LocalPlayer.NickName;
                instance = new OnlineLogging(username);
                instance.Write(string.Format("{0}'s LOG\n", username));
            }

            return instance;
        }
    }
    

    private void OnApplicationQuit()
    {
        Close();
    }

}
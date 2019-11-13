using System.IO;

public class OnlineLogging {

    private string player;
    private string path;
    private StreamWriter streamWriter;
    private bool closed;

    public OnlineLogging (string player) {
        this.player = player;
        this.path = string.Format ("./{0}.txt", player);
        this.streamWriter = new StreamWriter (this.path, false);
        this.closed = false;
    }

    public void Write (string text) {
        if (closed) {
            return;
        }

        string outputText = string.Format ("{0}: {1}\n", player, text);
        streamWriter.Write (outputText);
    }

    public void Close () {
        if (closed) {
            return;
        }

        streamWriter.Close ();
    }

}
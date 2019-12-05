using System;

public class TonguedUsernamesBank {

    private static Random rng = new Random();
    private static string[] usernames = new string[] {
        "U1", "U2", "U3", "U4", "U5", "U6"
    };

    public static string RetriveUsername() {
        int randomIndex = rng.Next(usernames.Length);
        return usernames[randomIndex];
    }

}
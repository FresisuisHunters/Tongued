using System;

public class TonguedUsernamesBank {

    private static Random rng = new Random();
    private static string[] usernames = new string[] {
        "Karmaeleon", "BigLengus", "TongueMcCurly", "Chromatix",
        "DrLenguestein", "LoudDjembe", "PsycoMandala", "Lenguante",
        "unkulunkule", "angrymonkey", "supongotongo", "simbatheking",
        "Kleonidae", "BobaToad", "DarthFurcifer", "Colorholic", "swingsawng",
        "NotAnIguana", "CforChameleon", "rainbowskin"
    };

    public static string RetrieveUsername() {
        int randomIndex = rng.Next(usernames.Length);
        return usernames[randomIndex];
    }

}
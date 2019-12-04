using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Esta clase se usa para guardar de forma simplificada las puntuaciones de los jugadores de cara a la pantalla de puntuaciones
/// </summary>
public class PlayerScoreData
{
    private string name;
    private int score;
    public PlayerScoreData(string name, int score)
    {
        this.name = name;
        this.score = score;
    }
    public string getName()
    {
        return name;
    }
    public int getScore()
    {
        return score;
    }
}

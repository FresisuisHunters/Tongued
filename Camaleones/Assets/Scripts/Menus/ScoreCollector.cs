using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Esta clase se usa para llevar los datos de las puntuaciones a la escena donde se muestran las mismas
/// </summary>
public class ScoreCollector : MonoBehaviour
{
    private List<PlayerScoreData> scores;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    /// <summary>
    /// Guardamos las puntuaciones de todos los jugadores, así si uno se desconecta no habrá problemas
    /// </summary>
    public void CollectScores()
    {
        Debug.Log("Collecting scores");
        scores = new List<PlayerScoreData>();
        foreach (ScoreHandler sh in FindObjectsOfType<ScoreHandler>())
        {
            scores.Add(new PlayerScoreData(sh.Name, sh.CurrentScore));
        }
    }

    /// <summary>
    /// devolvemos una lista con los nombres de los jugadores y sus puntuaciones
    /// </summary>
    /// <returns></returns>
    public List<PlayerScoreData> GetScores()
    {
        return scores;
    }
}

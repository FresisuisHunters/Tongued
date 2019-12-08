using System.Collections.Generic;
using TMPro;
using UnityEngine;

#pragma warning disable 649
public class ScoresScreen : MonoBehaviour
{
    #region Inspector Variables
    [Tooltip("Prefab del panel que se usa para cada puntuacion")]
    [SerializeField] private ScoreEntry scoreEntryPrefab;
    [Tooltip("Tiempo que tarda en volver a la sala automáticamente.")]
    [SerializeField] private int exitTime;

    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Transform entryParent;
    #endregion

    #region private variables
    private float startTime;
    #endregion

    private void Start()
    {
        ShowScores(FindObjectOfType<ScoreCollector>().GetScores());
        startTime = Time.time;
    }

    /// <summary>
    /// Este metodo recibe una lista de playerScore data que tienen el nombre y puntuacion de cada jugador, luego los ordena y instancia paneles para cada uno con su nombre y resultado
    /// </summary>
    /// <param name="scores"></param>
    public void ShowScores(List<PlayerScoreData> scores)
    {
        Debug.Log("Showing scores");

        scores.Sort((b, a) => a.getScore().CompareTo(b.getScore()));
        int currentPosition = 0;
        int previousEntryScore = int.MaxValue;

        foreach (PlayerScoreData scoreData in scores)
        {
            int score = scoreData.getScore();
            string name = scoreData.getName();
            if (score < previousEntryScore) currentPosition++;

            ScoreEntry entry = Instantiate(scoreEntryPrefab, entryParent);

            entry.SetScore(score);
            entry.SetName(name);
            entry.SetPosition(currentPosition);

            previousEntryScore = score;
        }

        // Destroy(FindObjectOfType<ScoreCollector>());
    }

    private void Update()
    {
        int timeLeft = (int)Mathf.Max(Mathf.Ceil(exitTime - (Time.time - startTime)),0);
        timerText.SetText(timeLeft.ToString());
        if (timeLeft <= 0)
            GoToRoom();
    }

    private void GoToRoom()
    {
        SceneManagerExtensions.LoadScene("sce_mLobby", UnityEngine.SceneManagement.LoadSceneMode.Single, () =>
        {
            MenuScreenManager manager = FindObjectOfType<MenuScreenManager>();
            manager.startingMenuScreen = FindObjectOfType<RoomScreen>();
        });
    }

    
}

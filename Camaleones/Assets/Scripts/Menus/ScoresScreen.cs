using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

#pragma warning disable 649
public class ScoresScreen : MonoBehaviour
{
    #region Inspector Variables
    [Tooltip("Prefab del panel que se usa para cada puntuacion")]
    [SerializeField] private GameObject scorePanelPrefab;
    [Tooltip("Tiempo que tarda en volver a la sala automáticamente.")]
    [SerializeField] private int exitTime;
    #endregion

    #region private variables
    private float startTime;
    private TextMeshProUGUI timerText;
    #endregion

    private void Start()
    {
        ShowScores(FindObjectOfType<ScoreCollector>().GetScores());
        startTime = Time.time;
        timerText = GameObject.Find("ExitTimer").GetComponent<TextMeshProUGUI>();
    }

    /// <summary>
    /// Este metodo recibe una lista de playerScore data que tienen el nombre y puntuacion de cada jugador, luego los ordena y instancia paneles para cada uno con su nombre y resultado
    /// </summary>
    /// <param name="scores"></param>
    public void ShowScores(List<PlayerScoreData> scores)
    {
        Debug.Log("Recibido: " + scores[0].getName());
        scores.Sort((b, a) => a.getScore().CompareTo(b.getScore()));
        int position = 1;

        for (int i = 0; i < scores.Count; i++)
        {
            GameObject scoreObject = Instantiate(scorePanelPrefab, transform);
            scoreObject.GetComponent<RectTransform>().localPosition = new Vector3(0, 400 - i * 100, 0);
            scoreObject.GetComponent<RectTransform>().localScale = Vector3.one;
            if(i>0)
            {
                if (scores[i].getScore() < scores[i - 1].getScore())
                    position++;
            }
            scoreObject.GetComponentInChildren<TextMeshProUGUI>().SetText(position + "º " + scores[i].getName() + " - " + Mathf.Max(scores[i].getScore(), 0));
        }

        Destroy(FindObjectOfType<ScoreCollector>());

        //Invoke("GoToRoom", exitTime);
        
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
            Debug.Log(FindObjectOfType<RoomScreen>());
            manager.startingMenuScreen = FindObjectOfType<RoomScreen>();
        });
    }

    
}

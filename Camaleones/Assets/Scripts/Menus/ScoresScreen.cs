using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ScoresScreen : MonoBehaviour
{
    [Tooltip("Prefab del panel que se usa para cada puntuacion")]
    [SerializeField] private GameObject scorePanelPrefab;
    
    /// <summary>
    /// Este metodo recibe una lista de playerScore data que tienen el nombre y puntuacion de cada jugador, luego los ordena y instancia paneles para cada uno con su nombre y resultado
    /// </summary>
    /// <param name="scores"></param>
    public void ShowScores(List<PlayerScoreData> scores)
    { 
        scores.Sort((b, a) => a.getScore().CompareTo(b.getScore()));
        
        for(int i = 0; i<scores.Count; i++)
        {
            GameObject scoreObject = Instantiate(scorePanelPrefab, transform);
            scoreObject.GetComponent<RectTransform>().localPosition = new Vector3(0, 450 - i * 100,0);
            scoreObject.GetComponent<RectTransform>().localScale = Vector3.one;
            scoreObject.GetComponentInChildren<TextMeshProUGUI>().SetText((i + 1) + "º " + scores[i].getName() + " - " + Mathf.Max(scores[i].getScore(),0));
        }
    }
}

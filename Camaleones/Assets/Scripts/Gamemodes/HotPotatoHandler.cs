using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

#pragma warning disable 649

/// <summary>
/// 
/// </summary>
[RequireComponent(typeof(PlayersHandler))]
public class HotPotatoHandler : MonoBehaviour
{
    #region Inspector
    [Tooltip("Numero de rondas que tendrá la partida")]
    [SerializeField] private int roundNumber;
    [Tooltip("Tiempo inicial que necesita tener un jugador la snitch para ganar. Se ira reduciendo durante la ronda")]
    [SerializeField] private float baseSnitchPossesionTimer;
    [Tooltip("Curva que define como se reduce el tiempo necesario de posesión de la snitch para cada ronda")]
    [SerializeField] private AnimationCurve posessionTimeCurve;
    [Tooltip("Prefab de la snitch")]
    [SerializeField] private GameObject snitch;
    #endregion

    #region Private Variables
    //Instante en el que empieza la ronda
    private float roundStartTime;
    //Instante en el que empieza el timer de la última posesión de la snitch
    private float timerStartTime;
    //tiempo final para el timer en un momento concreto. Se reevalua mirando posessionTimeCurve
    private float currentEndTime;
    //Timer que se muestra en pantalla
    private int timerCurrentTime;
    //
    private bool hasStarted;
    //
    private TextMeshProUGUI timerText;
    //Tipo de ronda, si se gana por coger la bola o por huir de ella
    //true = coger false = huir
    private bool roundType;
    //
    private int currentRound;
    //
    private PlayersHandler playersHandler;
    #endregion

    /// <summary>
    /// 
    /// </summary>
    void Awake()
    {
        GameObject spawnPoint = GameObject.FindGameObjectWithTag("SnitchSpawnPoint");
        if (!spawnPoint)
        {
            Debug.LogError("There is no SnitchSpawnPoint in the scene.");
        }

        GameObject snitchReference = Instantiate(snitch, spawnPoint.transform.position, Quaternion.identity);
        hasStarted = false;

        timerText = GetComponentInChildren<TextMeshProUGUI>();

        roundType = true;
        currentRound = 0;

        playersHandler = GetComponent<PlayersHandler>();
    }

    /// <summary>
    /// 
    /// </summary>
    public void NotifyTransfer()
    {
        if (!hasStarted)
        {
            hasStarted = true;
            roundStartTime = Time.time;
        }
        timerStartTime = Time.time;
        RestartTimer();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(hasStarted)
        {
            float timerTime = (currentEndTime - (Time.time - timerStartTime));
            UpdateTimer();
            if(timerTime<=0)
            {
                StartNewRound();
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void UpdateTimer()
    {
        float timerTime = Mathf.Ceil(Mathf.Max((currentEndTime - (Time.time - timerStartTime)), 0));
        timerText.SetText(timerTime.ToString());
    }

    /// <summary>
    /// 
    /// </summary>
    private void RestartTimer()
    {
        timerStartTime = Time.time;
        currentEndTime = posessionTimeCurve.Evaluate(Time.time - roundStartTime);
    }

    /// <summary>
    /// 
    /// </summary>
    private void StartNewRound()
    {
        if(currentRound==roundNumber-1)
        {

        }
        else
        {
            currentRound++;
            roundType = !roundType;
            playersHandler.SpawnPlayers();
        }
    }
}

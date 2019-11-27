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
    /*
    [Tooltip("Tiempo inicial que necesita tener un jugador la snitch para ganar. Se ira reduciendo durante la ronda")]
    [SerializeField] private float baseSnitchPossesionTimer;
    */
    [Tooltip("Curva que define como se reduce el tiempo necesario de posesión de la snitch para cada ronda")]
    [SerializeField] private AnimationCurve posessionTimeCurve;
    [Tooltip("Prefab de la snitch")]
    [SerializeField] private GameObject snitch;
    #endregion

    #region Private/protected Variables
    //Instante en el que empieza la ronda
    private float roundStartTime;
    //Instante en el que empieza el timer de la última posesión de la snitch
    protected float timerStartTime;
    //tiempo final para el timer en un momento concreto. Se reevalua mirando posessionTimeCurve
    protected float currentEndTime;
    //Timer que se muestra en pantalla
    protected int timerCurrentTime;
    //
    protected bool hasStarted;
    //
    protected TextMeshProUGUI timerText;
    //Tipo de ronda, si se gana por coger la bola o por huir de ella
    //true = coger false = huir
    protected bool roundType;
    //
    protected int currentRound;
    //
    protected float roundChangeChance;
    //
    protected PlayersHandler playersHandler;
    //
    protected TransferableItem snitchReference;
    #endregion

    /// <summary>
    /// 
    /// </summary>
    protected virtual void Awake()
    {
        GameObject spawnPoint = GameObject.FindGameObjectWithTag("SnitchSpawnPoint");
        if (!spawnPoint)
        {
            Debug.LogError("There is no SnitchSpawnPoint in the scene.");
        }

        snitchReference = Instantiate(snitch, spawnPoint.transform.position, Quaternion.identity).GetComponent<TransferableItem>();
        hasStarted = false;

        timerText = GetComponentInChildren<TextMeshProUGUI>();

        roundType = true;
        currentRound = 0;
        roundChangeChance = 0.5f;

        playersHandler = GetComponent<PlayersHandler>();
    }

    protected virtual void SpawnSnitch()
    {

    }

    /// <summary>
    /// 
    /// </summary>
    public virtual void NotifyTransfer()
    {
        if (!hasStarted)
        {
            hasStarted = true;
            roundStartTime = Time.time;
        }
        timerStartTime = Time.time;
        RestartTimer();
    }

    // Update is called once per frame
    protected virtual void Update()
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
    protected virtual void UpdateTimer()
    {
        timerCurrentTime = (int)Mathf.Ceil(Mathf.Max((currentEndTime - (Time.time - timerStartTime)), 0));
        timerText.SetText(timerCurrentTime.ToString());
    }

    /// <summary>
    /// 
    /// </summary>
    protected virtual void RestartTimer()
    {
        timerStartTime = Time.time;
        currentEndTime = posessionTimeCurve.Evaluate(Time.time - roundStartTime);
    }

    /// <summary>
    /// 
    /// </summary>
    protected virtual void StartNewRound()
    {
        if(currentRound==roundNumber-1)
        {
            //SE ACABA LA PARTIDA, NO SÉ QUÉ PONER AHORA MISMO
            Debug.Log("Se acabó wey");
        }
        else
        {
            currentRound++;
            
            if (roundType)
                snitchReference.AddScore(1);
            else
                snitchReference.AddScore(-1);

            float rnd = Random.value;

            if(rnd>roundChangeChance)
            {
                roundChangeChance = roundChangeChance / 2;
                Debug.Log("La ronda no cambia");
            }
            else
            {
                roundChangeChance = 0.5f;
                roundType = !roundType;
                Debug.Log("La ronda cambia");
            }

            roundStartTime = Time.time;
            RestartTimer();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

#pragma warning disable 649

/// <summary>
/// Clase que gestiona el modo de juego Hot Potato
/// </summary>
[RequireComponent(typeof(PlayersHandler))]
public class HotPotatoHandler : MonoBehaviour
{
    #region Inspector
    [Tooltip("Numero de rondas que tendrá la partida")]
    [SerializeField] private int roundNumber;
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
    //Bool para saber si se ha cogido la snitch por primera vez, empezando el juego
    protected bool hasStarted;
    //Textmesh del timer
    protected TextMeshProUGUI timerText;
    //Tipo de ronda, si se gana por coger la bola o por huir de ella
    //true = coger false = huir
    protected bool roundType;
    //numero de la ronda actual
    protected int currentRound;
    //probabilidades de cambiar de ronda al acabar la actual
    protected float roundChangeChance;
    //objeto que gestiona a los jugadores
    protected PlayersHandler playersHandler;
    //referencia al objeto snitch
    protected TransferableItem snitchReference;
    #endregion

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

    /// <summary>
    /// Este método es llamado por la snitch cada vez que cambia de usuario
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
    /// Este metodo actualiza el timer en pantalla
    /// </summary>
    protected virtual void UpdateTimer()
    {
        timerCurrentTime = (int)Mathf.Ceil(Mathf.Max((currentEndTime - (Time.time - timerStartTime)), 0));
        timerText.SetText(timerCurrentTime.ToString());
    }

    /// <summary>
    /// Este metodo reinicia el timer cada vez que la snitch cambia de persona
    /// </summary>
    protected virtual void RestartTimer()
    {
        timerStartTime = Time.time;
        currentEndTime = posessionTimeCurve.Evaluate(Time.time - roundStartTime);
    }

    /// <summary>
    /// Este método se llama cada vez que termina la ronda
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

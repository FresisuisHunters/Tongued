using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
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
    [SerializeField] private int numberOfRounds;
    [Tooltip("Curva que define como se reduce el tiempo necesario de posesión de la snitch para cada ronda")]
    [SerializeField] private AnimationCurve posessionTimeCurve;
    [Tooltip("Prefab de la snitch")]
    [SerializeField] private TransferableItem snitchPrefab;
    #endregion

    public event System.Action<RoundType> OnNewRound;

    #region Private state
    private float timeElapsedThisRound = 0;
    public float TimeLeftInRound { get; protected set; }
    public float RoundDurationSinceLastReset { get; private set; }

    /// <summary>
    /// Bool para saber si se ha cogido la snitch por primera vez, empezando el juego
    /// </summary>
    private bool firstRoundHasStarted = false;
    private RoundType currentRoundType;
    
    private int currentRoundNumber = 0;
    /// <summary>
    /// Probabilidades de cambiar de ronda al acabar la actual
    /// </summary>
    private float currentChanceOfSameRound = 0.5f;

    public TransferableItem Snitch { get; protected set; }
    #endregion

    protected virtual void StartRound(RoundType roundType)
    {
        firstRoundHasStarted = true;
        currentRoundNumber++;
        currentRoundType = roundType;

        OnNewRound?.Invoke(roundType);

        timeElapsedThisRound = 0;
        ResetTimer();
    }

    protected virtual void EndRound()
    {
        if (currentRoundNumber == numberOfRounds - 1)
        {
            EndMatch();
        }
        else
        {
            ScoreHandler scoreHandler = Snitch.CurrentHolder.GetComponent<ScoreHandler>();
            switch (currentRoundType)
            {
                case RoundType.Blessing:
                    scoreHandler.AddScore(1);
                    break;
                case RoundType.Curse:
                    scoreHandler.AddScore(-1);
                    break;
            }

            if (Random.value < currentChanceOfSameRound)
            {
                currentChanceOfSameRound = currentChanceOfSameRound / 2;
            }
            else
            {
                RoundType newRoundType;
                currentChanceOfSameRound = 0.5f;
                switch (currentRoundType)
                {
                    case RoundType.Blessing:
                        newRoundType = RoundType.Curse;
                        break;
                    case RoundType.Curse:
                        newRoundType = RoundType.Blessing;
                        break;
                    default:
                        //Nunca debería llegarse aquí, pero si no le doy un valor en el caso default el compilador se enfada.
                        newRoundType = RoundType.Blessing;
                        break;
                }
                StartRound(newRoundType);
            }
        }
    }

    protected virtual void EndMatch()
    {
        //SE ACABA LA PARTIDA, NO SÉ QUÉ PONER AHORA MISMO
        Debug.Log("Se acabó wey");
    }


    protected void OnSnitchTransfered(TransferableItemHolder newSnitchHolder)
    {
        if (!firstRoundHasStarted)
        {
            StartRound(RoundType.Blessing);
        }
        else
        {
            ResetTimer();
        }
    }

    private void Update()
    {
        if (firstRoundHasStarted)
        {
            timeElapsedThisRound += Time.deltaTime;
            TimeLeftInRound -= Time.deltaTime;
            
            if(TimeLeftInRound <= 0)
            {
                EndRound();
            }
        }
    }

    private void ResetTimer()
    {
        RoundDurationSinceLastReset = posessionTimeCurve.Evaluate(timeElapsedThisRound);
        TimeLeftInRound = RoundDurationSinceLastReset;
    }


    #region Initialization
    protected virtual void Awake()
    {
        SpawnSnitch();
    }

    private void SpawnSnitch()
    {
        GameObject spawnPoint = GameObject.FindGameObjectWithTag("SnitchSpawnPoint");
        if (!spawnPoint)
        {
            Debug.LogError("There is no SnitchSpawnPoint in the scene.");
        }

        Snitch = Instantiate(snitchPrefab, spawnPoint.transform.position, Quaternion.identity);
        Snitch.OnItemTransfered += OnSnitchTransfered;
    }
    #endregion

    public enum RoundType
    {
        Blessing,
        Curse
    }
}

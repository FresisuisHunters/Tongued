using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#pragma warning disable 649
/// <summary>
/// Clase que gestiona el modo de juego Hot Potato
/// </summary>
[RequireComponent(typeof(PlayersHandler))]
public class HotPotatoHandler : MonoBehaviour
{
    #region Inspector
    public float matchStartCountdownLength = 5f;

    [Tooltip("Numero de rondas que tendrá la partida")]
    [SerializeField] private int numberOfRounds;
    [Tooltip("Curva que define como se reduce el tiempo necesario de posesión de la snitch para cada ronda")]
    [SerializeField] private AnimationCurve posessionTimeCurve;
    [Tooltip("Prefab de la snitch")]
    [SerializeField] protected TransferableItem snitchPrefab;
    [Tooltip("Prefab del objeto que guarda las puntuaciones al final.")]
    [SerializeField] protected GameObject scoreCollector;
    [Tooltip("Escena de puntuaciones")]
    [SerializeField] protected string scoreSceneName;
    #endregion

    public int TotalRoundCount => numberOfRounds;

    public event System.Action OnSnitchActivated;
    public event System.Action<RoundType> OnNewRound;
    public event System.Action<TransferableItemHolder, TransferableItemHolder> OnSnitchTransfered;

    #region Private state
    public bool SnitchHasActivated { get; protected set; }
    public float TimeBeforeSnitchActivation { get; protected set; }

    private float timeElapsedThisRound = 0;
    public float TimeLeftInRound { get; protected set; }
    public float RoundDurationSinceLastReset { get; protected set; }

    public RoundType CurrentRoundType { get; private set; }
    
    public int CurrentRoundNumber { get; private set; }
    /// <summary>
    /// Probabilidades de cambiar de ronda al acabar la actual
    /// </summary>
    protected float currentChanceOfSameRound = 0.5f;

    public TransferableItem Snitch { get; protected set; }
    #endregion

    protected virtual void StartRound(RoundType roundType)
    {
        CurrentRoundNumber++;
        CurrentRoundType = roundType;

        OnNewRound?.Invoke(roundType);

        timeElapsedThisRound = 0;
        ResetTimer();
    }

    protected virtual void EndRound()
    {
        ScoreHandler scoreHandler = Snitch?.CurrentHolder?.GetComponent<ScoreHandler>();
        if (scoreHandler)
        {
            switch (CurrentRoundType)
            {
                case RoundType.Blessing:
                    scoreHandler.AddScore(1);
                    break;
                case RoundType.Curse:
                    scoreHandler.AddScore(-1);
                    break;
            }
        }

        if (CurrentRoundNumber == numberOfRounds)
        {
            EndMatch();
        }
        else
        {            
            RoundType newRoundType;
            if (Random.value < currentChanceOfSameRound)
            {
                currentChanceOfSameRound /= 2;
                newRoundType = CurrentRoundType;
            }
            else
            {
                currentChanceOfSameRound = 0.5f;
                switch (CurrentRoundType)
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
            }

            StartRound(newRoundType);
        }
    }

    protected virtual void EndMatch()
    {
        ScoreCollector scollector = Instantiate(scoreCollector).GetComponent<ScoreCollector>();
        scollector.CollectScores();
        
        SceneManager.LoadScene(scoreSceneName);
    }


    protected void OnSnitchTransferedHandler(TransferableItemHolder oldSnitchHolder, TransferableItemHolder newSnitchHolder)
    {
        ResetTimer();

        //Provoca las reacciones de los jugadores
        PlayerReactionType oldHolderReaction;
        PlayerReactionType newHolderReaction;
        switch (CurrentRoundType)
        {
            case RoundType.Blessing:
                oldHolderReaction = PlayerReactionType.Negative;
                newHolderReaction = PlayerReactionType.Positive;
                break;
            case RoundType.Curse:
                oldHolderReaction = PlayerReactionType.Positive;
                newHolderReaction = PlayerReactionType.Negative;
                break;
            default:
                //Nunca debería llegarse aquí.
                oldHolderReaction = PlayerReactionType.Negative;
                newHolderReaction = PlayerReactionType.Positive;
                break;
        }

        IPlayerReactionListener[] listeners;
        if (oldSnitchHolder)
        {
            listeners = oldSnitchHolder.GetComponents<IPlayerReactionListener>();
            for (int i = 0; i < listeners.Length; i++)
            {
                listeners[i].DoReaction(oldHolderReaction);
            }
        }
        if (newSnitchHolder)
        {
            listeners = newSnitchHolder.GetComponents<IPlayerReactionListener>();
            for (int i = 0; i < listeners.Length; i++)
            {
                listeners[i].DoReaction(newHolderReaction);
            }
        }

        OnSnitchTransfered?.Invoke(oldSnitchHolder, newSnitchHolder);
    }

    protected void Update()
    {
        if (!SnitchHasActivated)
        {
            TimeBeforeSnitchActivation -= Time.deltaTime;
            if (TimeBeforeSnitchActivation <= 0)
            {
                ActivateSnitch();
                StartRound(RoundType.Blessing);
            }
        }
        else
        {
            timeElapsedThisRound += Time.deltaTime;
            TimeLeftInRound -= Time.deltaTime;

            if (TimeLeftInRound <= 0)
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

        SnitchHasActivated = false;
        TimeBeforeSnitchActivation = matchStartCountdownLength;
    }

    protected void SpawnSnitch()
    {
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SnitchSpawnPoint");
        int choice = Random.Range(0, spawnPoints.Length);
        if (spawnPoints.Length == 0)
        {
            OnlineLogging.Instance.Write("There is no SnitchSpawnPoint in the scene.");
            Debug.LogError("There is no SnitchSpawnPoint in the scene.");
        }

        Snitch = Instantiate(snitchPrefab, spawnPoints[choice].transform.position, Quaternion.identity);
        Snitch.OnItemTransfered += OnSnitchTransferedHandler;

        Snitch.gameObject.SetActive(false);
    }

    protected virtual void ActivateSnitch()
    {
        SnitchHasActivated = true;
        Snitch.gameObject.SetActive(true);

        OnSnitchActivated?.Invoke();
    }

    #endregion

    public enum RoundType
    {
        Blessing,
        Curse
    }
}

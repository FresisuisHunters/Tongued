using UnityEngine;

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
    public float RoundDurationSinceLastReset { get; protected set; }

    /// <summary>
    /// Bool para saber si se ha cogido la snitch por primera vez, empezando el juego
    /// </summary>
    public bool FirstRoundHasStarted => CurrentRoundNumber > 0;
    public RoundType CurrentRoundType { get; private set; }
    
    public int CurrentRoundNumber { get; private set; }
    /// <summary>
    /// Probabilidades de cambiar de ronda al acabar la actual
    /// </summary>
    private float currentChanceOfSameRound = 0.5f;

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
        if (CurrentRoundNumber == numberOfRounds - 1)
        {
            EndMatch();
        }
        else
        {
            if (Snitch != null) {
                ScoreHandler scoreHandler = Snitch.CurrentHolder.GetComponent<ScoreHandler>();
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
        //SE ACABA LA PARTIDA, NO SÉ QUÉ PONER AHORA MISMO
        Debug.Log("Se acabó wey");
    }


    protected void OnSnitchTransfered(TransferableItemHolder oldSnitchHolder, TransferableItemHolder newSnitchHolder)
    {
        if (!FirstRoundHasStarted)
        {
            StartRound(RoundType.Blessing);
        }
        else
        {
            ResetTimer();
        }

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
    }

    protected void Update()
    {
        if (FirstRoundHasStarted)
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

    protected void SpawnSnitch()
    {
        GameObject spawnPoint = GameObject.FindGameObjectWithTag("SnitchSpawnPoint");
        if (!spawnPoint)
        {
            OnlineLogging.Instance.Write("There is no SnitchSpawnPoint in the scene.");
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

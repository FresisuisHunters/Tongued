using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 649

/// <summary>
/// 
/// </summary>
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
    private bool hasStarted;
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
        snitchReference.GetComponent<TransferableItem>().hotPotatoHandler = this;
        hasStarted = false;
        roundStartTime = Time.time;
    }

    /// <summary>
    /// 
    /// </summary>
    public void NotifyTransfer()
    {
        timerStartTime = Time.time;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void UpdateTimer()
    {

    }

    private void RestartTimer()
    {
        timerStartTime = Time.time;
        currentEndTime = posessionTimeCurve.Evaluate(Time.time - roundStartTime);
    }
}

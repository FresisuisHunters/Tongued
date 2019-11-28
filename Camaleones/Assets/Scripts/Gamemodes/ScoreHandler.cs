using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 649

public class ScoreHandler : MonoBehaviour
{

    #region Private Variables
    [Tooltip("Puntuacion del jugador")]
    [SerializeField]private int score;
    #endregion
    // Start is called before the first frame update
    protected virtual void Awake()
    {
        score = 0;
    }

    /// <summary>
    /// metodo que añade la puntuacion recibida al jugador
    /// </summary>
    /// <param name="score"></param>
    virtual public void AddScore(int score)
    {
        this.score += score;
    }
}

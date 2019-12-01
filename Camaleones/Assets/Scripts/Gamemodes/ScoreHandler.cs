using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 649

public class ScoreHandler : MonoBehaviour
{
    public int CurrentScore { get; private set; }
    
    
    protected virtual void Awake()
    {
        CurrentScore = 0;
    }

    /// <summary>
    /// Método que añade la puntuacion recibida al jugador
    /// </summary>
    virtual public void AddScore(int score)
    {
        this.CurrentScore += score;
    }
}

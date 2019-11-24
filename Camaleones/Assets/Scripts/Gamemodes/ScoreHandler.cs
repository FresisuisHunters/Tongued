using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 649

public class ScoreHandler : MonoBehaviour
{

    #region Private Variables
    [Tooltip("")]
    [SerializeField]private int score;
    #endregion
    // Start is called before the first frame update
    void Awake()
    {
        score = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddScore(int score)
    {
        this.score += score;
    }
}

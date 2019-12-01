using UnityEngine;

public class ScoreHandler : MonoBehaviour
{
    public int CurrentScore {
        get => _currentScore;
        protected set
        {
            _currentScore = value;
            OnScoreChanged?.Invoke(_currentScore);
        }
    }
    private int _currentScore;

    public event System.Action<int> OnScoreChanged;


    public virtual void AddScore(int diff)
    {
        CurrentScore += diff;
    }
}

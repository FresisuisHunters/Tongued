using UnityEngine;

public class ScoreHandler : MonoBehaviour
{
    public int CurrentScore {
        get => _currentScore;
        protected set
        {
            float diffSign = Mathf.Sign(value - _currentScore);
            _currentScore = value;
            OnScoreChanged?.Invoke(_currentScore);

            if (diffSign > 0) TriggerReaction(PlayerReactionType.Positive);
            else if (diffSign < 0) TriggerReaction(PlayerReactionType.Negative);
        }
    }
    private int _currentScore;

    public string Name { get; private set; }

    public event System.Action<int> OnScoreChanged;


    public virtual void AddScore(int diff)
    {
        CurrentScore += diff;
    }

    protected void TriggerReaction(PlayerReactionType reactionType)
    {
        IPlayerReactionListener[] listeners = GetComponents<IPlayerReactionListener>();
        for (int i = 0; i < listeners.Length; i++)
        {
            listeners[i].DoReaction(reactionType);
        }
    }

    private void Start()
    {
        Name = GetComponent<Photon.Pun.PhotonView>()?.Owner?.NickName ?? gameObject.name;
    }
}

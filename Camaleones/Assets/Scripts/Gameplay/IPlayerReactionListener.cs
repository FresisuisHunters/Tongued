public interface IPlayerReactionListener
{
    void DoReaction(PlayerReactionType reactionType);
}

public enum PlayerReactionType
{
    Positive,
    Negative
}
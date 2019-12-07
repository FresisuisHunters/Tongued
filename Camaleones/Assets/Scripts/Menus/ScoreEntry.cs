using UnityEngine;
using TMPro;

#pragma warning disable 649
public class ScoreEntry : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI positionText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI nameText;

    public void SetPosition(int position)
    {
        positionText.text = position.ToString() + ".";
    }

    public void SetScore(int score)
    {
        scoreText.text = score.ToString();
    }

    public void SetName(string name)
    {
        nameText.text = name;
    }
}

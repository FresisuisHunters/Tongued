using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshPro))]
public class PlayerNameAndScoreDisplay : MonoBehaviour
{
    private string playerName;
    private Vector3 positionOffset;

    private TextMeshPro text;


    private void SetDisplayText(string playerName, bool showScore, int score = 0)
    {
        string finalText = playerName + ": " + score.ToString();
        text.text = finalText;
    }

    private void LateUpdate()
    {
        // Para que no se mueva/rote el texto junto con el camaleon
        transform.rotation = Quaternion.identity;
        transform.position = transform.parent.position - positionOffset;
    }


    private void Awake()
    {
        text = GetComponent<TextMeshPro>();

        positionOffset = transform.parent.position - transform.position;
    }

    private void Start()
    {
        Photon.Pun.PhotonView photonView = GetComponentInParent<Photon.Pun.PhotonView>();
        if (photonView)
        {
            playerName = photonView.Owner.NickName;
            text.color = Color.yellow;
        }
        else playerName = transform.root.name;

        ScoreHandler scoreHandler = GetComponentInParent<ScoreHandler>();
        if (scoreHandler)
        {
            scoreHandler.OnScoreChanged += (int newScore) => SetDisplayText(playerName, true, newScore);
        }

        if (scoreHandler) SetDisplayText(playerName, true, scoreHandler.CurrentScore);
        else SetDisplayText(playerName, false);
    }
}

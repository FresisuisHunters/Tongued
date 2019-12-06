using UnityEngine;
using TMPro;
using Photon.Pun;

[RequireComponent(typeof(TextMeshPro))]
public class PlayerNameAndScoreDisplay : MonoBehaviour
{
    public static readonly Color LOCAL_PLAYER_COLOR = Color.yellow;
    public static readonly Color REMOTE_PLAYER_COLOR = Color.magenta;

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
        if (!photonView) {
            // Por si se llegase a usar en entrenamiento
            playerName = "You";
            text.color = LOCAL_PLAYER_COLOR;
        } else {
            bool photonViewBelongsToLocalPlayer = photonView.Owner.NickName.Equals(PhotonNetwork.LocalPlayer.NickName);
            playerName = photonView.Owner.NickName;
            text.color = (photonViewBelongsToLocalPlayer) ? LOCAL_PLAYER_COLOR : REMOTE_PLAYER_COLOR;
        }

        ScoreHandler scoreHandler = GetComponentInParent<ScoreHandler>();
        if (scoreHandler)
        {
            scoreHandler.OnScoreChanged += (int newScore) => SetDisplayText(playerName, true, newScore);
        }

        if (scoreHandler) SetDisplayText(playerName, true, scoreHandler.CurrentScore);
        else SetDisplayText(playerName, false);
    }
}

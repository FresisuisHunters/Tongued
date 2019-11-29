using Photon.Pun;
using UnityEngine;
using TMPro;

public class OnlineGameManager : MonoBehaviour {

    #region Public Members

    public GameObject playerPrefab;
    public GameObject nicknameOnScreenPrefab;

    #endregion

    #region Unity Callbacks

    protected void Awake () {
        InstantiatePlayer ();
    }

    protected void OnApplicationQuit() {
        OnlineLogging.Instance.Close();
    }

    #endregion

    #region Private Methods

    private void InstantiatePlayer () {
        float positionX = Random.Range (-5.0f, 5.0f);
        float positionY = playerPrefab.transform.position.y;

        Vector2 position = new Vector2 (positionX, positionY);
        Quaternion rotation = playerPrefab.transform.rotation;

        GameObject player = PhotonNetwork.Instantiate (playerPrefab.name, position, rotation, 0);
        player.name = PhotonNetwork.LocalPlayer.NickName;

        GameObject nicknameCanvas = GameObject.FindGameObjectWithTag("NicknameCanvas");
        GameObject nicknameOnScreen = PhotonNetwork.Instantiate(nicknameOnScreenPrefab.name, position, rotation, 0);
        nicknameOnScreen.transform.SetParent(nicknameCanvas.transform, false);

        OnlinePlayer playerScript = player.GetComponent<OnlinePlayer>();
        TextMeshProUGUI playerNicknameText = nicknameOnScreen.GetComponent<TextMeshProUGUI>();
        playerScript.NicknameOnCanvas = nicknameOnScreen.GetComponent<TextMeshProUGUI>();
        playerNicknameText.text = player.name;
    }

    #endregion
}
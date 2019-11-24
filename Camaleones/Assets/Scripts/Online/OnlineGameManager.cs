using Photon.Pun;
using UnityEngine;

public class OnlineGameManager : MonoBehaviour {

    #region Public Members

    public GameObject playerPrefab;

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
        Debug.Log("Jugador instanciado por " + PhotonNetwork.LocalPlayer.NickName);

        float positionX = Random.Range (-5.0f, 5.0f);
        float positionY = playerPrefab.transform.position.y;

        string prefabName = playerPrefab.name;
        Vector2 position = new Vector2 (positionX, positionY);
        Quaternion rotation = playerPrefab.transform.rotation;

        GameObject player = PhotonNetwork.Instantiate (prefabName, position, rotation, 0);
        player.name = PhotonNetwork.LocalPlayer.NickName;
    }

    #endregion
}
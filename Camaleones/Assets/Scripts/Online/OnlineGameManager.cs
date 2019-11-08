using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class OnlineGameManager : MonoBehaviourPunCallbacks {

    #region Public Members

    public GameObject playerPrefab;

    #endregion

    #region Private Members

    private byte numberOfPlayers;

    #endregion

    #region Unity Callbacks

    private void Awake () {
        numberOfPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
    }

    private void Start () {
        InstantiatePlayer ();
    }

    #endregion

    #region Private Methods

    private void InstantiatePlayer () {
        float positionX = Random.Range (-5.0f, 5.0f);
        float positionY = playerPrefab.transform.position.y;

        string prefabName = playerPrefab.name;
        Vector2 position = new Vector2 (positionX, positionY);
        Quaternion rotation = playerPrefab.transform.rotation;

        PhotonNetwork.Instantiate (prefabName, position, rotation, 0);
    }

    #endregion

}
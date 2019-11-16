using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Componente que inicializa el modo de juego de patata caliente. 
/// No debería formar parte de la escena. Al cargar una partida de este modo, debería instanciarse este prefab para inicializar el mapa con este modo.
/// </summary>
public class HotPotatoModeInitializer : MonoBehaviour
{
    public TransferableItem snitch;

    private void Awake()
    {
        GameObject spawnPoint = GameObject.FindGameObjectWithTag("SnitchSpawnPoint");
        if (!spawnPoint)
        {
            Debug.LogError("There is no SnitchSpawnPoint in the scene.");
        }

        Instantiate(snitch, spawnPoint.transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}

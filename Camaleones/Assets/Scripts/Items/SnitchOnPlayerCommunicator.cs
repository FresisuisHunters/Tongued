using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Esta clase solo sirve para comunicar las colisiones del jugador al objeto Snitch si lo tiene
/// </summary>
public class SnitchOnPlayerCommunicator : MonoBehaviour
{
    /// <summary>
    /// Este método transfiere las colisiones a la clase del objeto transferible para que pueda gestionarla
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        GetComponentInChildren<TransferableItem>().Collide(collision.gameObject);
    }
}

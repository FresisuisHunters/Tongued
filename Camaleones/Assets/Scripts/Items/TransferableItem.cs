using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 649

/// <summary>
/// Este script define un objeto que se transfiere entre jugadores por contacto, o al tocarlo cuando no lo lleva un jugador, al principio de la partida.
/// </summary>
public class TransferableItem : MonoBehaviour
{
    #region Inspector
    [Tooltip("Tiempo que tiene que pasar desde que un jugador adquiere el objeto hasta que otro jugador puede quitarselo")]
    [SerializeField] private float cooldownToTransfer;
    #endregion

    protected bool transferActive;

    protected virtual void Awake()
    {
        transferActive = true;
    }

    /// <summary>
    /// Corrutina que se usa para el temporizador que activa la transferencia del objeto
    /// </summary>
    /// <returns></returns>
    IEnumerator ActivationTimer()
    {
        yield return new WaitForSeconds(cooldownToTransfer);
        transferActive = true;
    }

    /// <summary>
    /// Al tocar un jugador o su lengua si la transferencia está activa le añade un componente de este tipo y se autodestruye, quitandolo del objeto que lo tenia antes
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Collide(collision.gameObject);
    }

    /// <summary>
    /// Al tocar un jugador o su lengua si la transferencia está activa le añade un componente de este tipo y se autodestruye, quitandolo del objeto que lo tenia antes
    /// </summary>
    /// <param name="collision"></param>
    public virtual void Collide(GameObject collisionObject)
    {
        if (transferActive)
        {
            if (collisionObject.layer == LayerMask.NameToLayer("MainPlayerLayer") || collisionObject.layer == LayerMask.NameToLayer("PlayerLayer"))
            {
                TITransfer(collisionObject);
            }
        }
    }

    protected virtual void TITransfer(GameObject target)
    {
        Debug.Log(target);
        Destroy(GetComponentInParent<SnitchOnPlayerCommunicator>());
        transform.SetParent(target.transform);
        target.AddComponent<SnitchOnPlayerCommunicator>();
        GetComponent<Collider2D>().enabled = false;
        transferActive = false;
        StartCoroutine(ActivationTimer());
    }
}

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

    public event System.Action<TransferableItemHolder, TransferableItemHolder> OnItemTransfered;

    public TransferableItemHolder CurrentHolder { get; private set; }

    protected bool transferActive = true;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        Collide(collision.gameObject);
    }

    /// <summary>
    /// Comprueba si ha colisionado con un jugador y si es así llama al método que transfiere el objeto a dicho jugador
    /// </summary>
    public virtual void Collide(GameObject collisionObject)
    {
        if (transferActive)
        {
            TransferableItemHolder newHolder = collisionObject.GetComponent<TransferableItemHolder>();
            if (newHolder) TITransfer(newHolder);
        }
    }

    /// <summary>
    /// Transfiere el objeto a otro jugador
    /// </summary>
    protected virtual void TITransfer(TransferableItemHolder newHolder)
    {
        TransferableItemHolder oldHolder = CurrentHolder;
        CurrentHolder = newHolder;
        if (oldHolder) oldHolder.item = null;
        
        transform.SetParent(newHolder.ItemParent);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        CurrentHolder.item = this;

        GetComponent<Collider2D>().enabled = false;
        transferActive = false;
        StartCoroutine(ActivationTimer());

        OnItemTransfered?.Invoke(oldHolder, newHolder);
    }

    /// <summary>
    /// Corrutina que se usa para el temporizador que activa la transferencia del objeto
    /// </summary>
    IEnumerator ActivationTimer()
    {
        yield return new WaitForSeconds(cooldownToTransfer);
        transferActive = true;
    }
}

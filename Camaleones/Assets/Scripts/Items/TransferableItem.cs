using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

#pragma warning disable 649


/// <summary>
/// Este script define un objeto que se transfiere entre jugadores por contacto, o al tocarlo cuando no lo lleva un jugador, al principio de la partida.
/// </summary>
[RequireComponent(typeof(ParentConstraint))]
public class TransferableItem : MonoBehaviour
{
    #region Inspector
    [Tooltip("Tiempo que tiene que pasar desde que un jugador adquiere el objeto hasta que otro jugador puede quitarselo")]
    [SerializeField] private float cooldownToTransfer;
    #endregion

    public event System.Action<TransferableItemHolder, TransferableItemHolder> OnItemTransfered;

    public TransferableItemHolder CurrentHolder { get; private set; }

    private ParentConstraint parentConstraint;
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

    public void SetToNoHolder()
    {
        TITransfer(null);
    }

    /// <summary>
    /// Transfiere el objeto a otro jugador
    /// </summary>
    protected virtual void TITransfer(TransferableItemHolder newHolder)
    {
        TransferableItemHolder oldHolder = CurrentHolder;
        CurrentHolder = newHolder;
        if (oldHolder) oldHolder.item = null;

        SetParentConstraint(newHolder?.ItemParent);
        if (newHolder) CurrentHolder.item = this;

        GetComponent<Collider2D>().enabled = !newHolder;
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

    private void SetParentConstraint(Transform newParent)
    {
        for (int i = parentConstraint.sourceCount - 1; i >= 0; i--)
        {
            parentConstraint.RemoveSource(i);
        }

        if (newParent)
        {
            ConstraintSource constraint = new ConstraintSource();
            constraint.sourceTransform = newParent;
            constraint.weight = 1;
            parentConstraint.AddSource(constraint);

            parentConstraint.SetTranslationOffset(0, Vector3.zero);
            parentConstraint.SetRotationOffset(0, Vector3.zero);
            
            parentConstraint.weight = 1;
            parentConstraint.enabled = true;
        }
        else
        {
            parentConstraint.enabled = false;
        }
    }

    protected virtual void Awake()
    {
        parentConstraint = GetComponent<ParentConstraint>();
        parentConstraint.enabled = false;
    }
}

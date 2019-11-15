using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransferableItem : MonoBehaviour
{
    #region Inspector
    [Tooltip("Tiempo que tiene que pasar desde que un jugador adquiere el objeto hasta que otro jugador puede quitarselo")]
    [SerializeField] private float cooldownToTransfer;
    #endregion

    private bool transferActive;

    public void Awake()
    {
        Debug.Log("I'm in " + gameObject);
        transferActive = false;
        StartCoroutine(ActivationTimer());
    }

    /// <summary>
    /// Corrutina que se usa para el temporizador que activa la transferencia del objeto
    /// </summary>
    /// <returns></returns>
    IEnumerator ActivationTimer()
    {
        yield return new WaitForSeconds(cooldownToTransfer);
        transferActive = true;
        if (GetComponent<ItemSelfDestroy>())
            GetComponent<ItemSelfDestroy>().active = true;
    }

    /// <summary>
    /// Al tocar un jugador o su lengua si la transferencia está activa le añade un componente de este tipo y se autodestruye, quitandolo del objeto que lo tenia antes
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (transferActive)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("HookLayer"))
            {
                GameObject transferDestination = collision.GetComponentInParent<Hook>().getParent();
                Debug.Log(transferDestination);
                transferDestination.AddComponent<TransferableItem>();
                transferDestination.GetComponent<TransferableItem>().cooldownToTransfer = this.cooldownToTransfer;
                Destroy(this);
            }
            else if (collision.gameObject.layer == LayerMask.NameToLayer("MainPlayerLayer") || collision.gameObject.layer == LayerMask.NameToLayer("PlayerLayer"))
            {
                GameObject transferDestination = collision.gameObject;
                Debug.Log(transferDestination);
                transferDestination.AddComponent<TransferableItem>();
                transferDestination.GetComponent<TransferableItem>().cooldownToTransfer = this.cooldownToTransfer;
                Destroy(this);
            }
        }
    }

    /// <summary>
    /// Al tocar un jugador o su lengua si la transferencia está activa le añade un componente de este tipo y se autodestruye, quitandolo del objeto que lo tenia antes
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (transferActive)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("HookLayer"))
            {
                GameObject transferDestination = collision.gameObject.GetComponentInParent<Hook>().getParent();
                Debug.Log(transferDestination);
                transferDestination.AddComponent<TransferableItem>();
                transferDestination.GetComponent<TransferableItem>().cooldownToTransfer = this.cooldownToTransfer;
                Destroy(this);
            }
            else if (collision.gameObject.layer == LayerMask.NameToLayer("MainPlayerLayer") || collision.gameObject.layer == LayerMask.NameToLayer("PlayerLayer"))
            {
                GameObject transferDestination = collision.gameObject;
                Debug.Log(transferDestination);
                transferDestination.AddComponent<TransferableItem>();
                transferDestination.GetComponent<TransferableItem>().cooldownToTransfer = this.cooldownToTransfer;
                Destroy(this);
            }
        }
    }
}

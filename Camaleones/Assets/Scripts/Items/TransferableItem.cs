using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransferableItem : MonoBehaviour
{
    [SerializeField] private float cooldownToTransfer;

    private bool transferActive;

    public void Awake()
    {
        Debug.Log("I'm in " + gameObject);
        transferActive = false;
        StartCoroutine(ActivationTimer());
    }

    IEnumerator ActivationTimer()
    {
        yield return new WaitForSeconds(cooldownToTransfer);
        transferActive = true;
        if (GetComponent<ItemSelfDestroy>())
            GetComponent<ItemSelfDestroy>().active = true;
    }

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

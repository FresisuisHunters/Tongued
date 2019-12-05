using UnityEngine;

#pragma warning disable 649
public class TransferableItemHolder : MonoBehaviour
{
    public Transform ItemParent => itemParent;
    [SerializeField] private Transform itemParent;

    [System.NonSerialized] public TransferableItem item;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        item?.Collide(collision.transform.root.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        item?.Collide(collision.transform.root.gameObject);
    }

    private void OnDestroy()
    {
        if (item) item.SetToNoHolder();
    }
}

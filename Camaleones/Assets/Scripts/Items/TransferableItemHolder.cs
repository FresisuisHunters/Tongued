using UnityEngine;

#pragma warning disable 649
public class TransferableItemHolder : MonoBehaviour
{
    public Transform ItemParent => itemParent;
    [SerializeField] private Transform itemParent;

    public bool hasItem;
}

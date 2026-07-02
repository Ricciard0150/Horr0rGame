using UnityEngine;

public enum ItemType
{
    Battery,
    Key,
    RightCarDoor,
    LeftCarDoor,
    Hammer,
    KeyCar,
}
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Outline))]
public class PickUpItems : MonoBehaviour, ICollectable
{
    [SerializeField] private ItemType itemType;
    public ItemType ItemType => itemType;

    private Rigidbody rb;
    private BoxCollider col;
    private Outline outline;

    private bool isCollected;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<BoxCollider>();
        outline = GetComponent<Outline>();

        HideOutline();
    }

    public void Collect(Transform parent)
    {
        if (Inventory.Instance.CurrentItem != null)
        {
            Inventory.Instance.CurrentItem.Drop();
        }

        isCollected = true;

        rb.isKinematic = true;
        col.enabled = false;

        transform.SetParent(parent);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        Inventory.Instance.SetItem(this);
    }

    public void Drop()
    {
        if (!isCollected) return;

        isCollected = false;

        transform.SetParent(null);

        rb.isKinematic = false;
        col.enabled = true;

        Inventory.Instance.ClearItem();
    }
    public void ShowOutline()
    {
        if (outline != null)
            outline.enabled = true;
    }

    public void HideOutline()
    {
        if (outline != null)
            outline.enabled = false;
    }
}
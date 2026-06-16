using UnityEngine;

public class PickUpItems : MonoBehaviour
{
    [SerializeField] private GameObject _item;
    [SerializeField] private Transform _itemParent;

    private Rigidbody rb;
    private MeshCollider meshCollider;

    private void Start()
    {
        rb = _item.GetComponent<Rigidbody>();
        meshCollider = _item.GetComponent<MeshCollider>();

        rb.isKinematic = true;
    }

    void Drop()
    {
        _item.transform.SetParent(null);

        rb.isKinematic = false;
        meshCollider.enabled = true;

        // Mantém a rotação atual
        _item.transform.rotation = Quaternion.identity;
    }

    void EquipItem()
    {
        _item.transform.SetParent(_itemParent);
        _item.transform.localPosition = Vector3.zero;
        _item.transform.localRotation = Quaternion.identity;

        rb.isKinematic = true;
        meshCollider.enabled = false;
    }
}
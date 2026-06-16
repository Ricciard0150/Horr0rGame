using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
[RequireComponent(typeof(Outline))]

public class PickUpItems : MonoBehaviour, ICollectable
{
    [SerializeField] private GameObject _item;
    [SerializeField] private Transform _itemParent;
    private Outline _outline;

    private Rigidbody rb;
    private BoxCollider boxCollider;

    private void Start()
    {
        rb = _item.GetComponentInChildren<Rigidbody>();
        boxCollider = _item.GetComponent<BoxCollider>();

        rb.isKinematic = true;
    }
    public 

    void Drop()
    {
        _itemParent.DetachChildren();
        _item.transform.eulerAngles = new Vector3(_item.transform.position.x, _item.transform.position.y, _item.transform.position.z);
        GetComponentInChildren<Rigidbody>().isKinematic = false;
        GetComponent<BoxCollider>().enabled = true;
    }



    public void Collect()
    {
       _item.GetComponentInChildren<Rigidbody>().isKinematic = true;

        _item.transform.position = _itemParent.transform.position;
        _item.transform.rotation = _itemParent.transform.rotation;

        _item.GetComponent<BoxCollider>().enabled = false;

        _item.transform.SetParent(_itemParent);
    }
    public void OnDrop(InputValue value)
    {
        Drop();
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (!collision.gameObject.TryGetComponent(out IStatusPlayer status))
            return;

        Collect();

    }

    public void ShowOutline()
    {
        if (_outline != null)
        {
            _outline.enabled = true;
        }
    }

    public void HideOutline()
    {
        if (_outline != null)
        {
            _outline.enabled = true;
        }
    }
}
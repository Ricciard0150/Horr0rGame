using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Outline))]
public class PlankDoor : MonoBehaviour, IInteractable
{
    private Outline _outline;
    private Rigidbody _rb;
    private BoxCollider boxCollider;

    [SerializeField] PickUpItems _pui;
    public ItemType itemType;
    public void Interact()
    {
        if (!_pui.isCollected)
            return;
             _rb.isKinematic = false;
             boxCollider.enabled = false;
        
    }
    public void HideOutline()
    {
        if (_outline != null)
        {
            _outline.enabled = false;
        }
    }

    public void ShowOutline()
    {
        if (_outline != null)
        {
            _outline.enabled = true;
        }
    }

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        _outline = GetComponentInChildren<Outline>();
        _outline.enabled = false;
    }
    public void OnInteract(InputValue value)
    {
        Interact();
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Outline))]
public class PuzzleCar : MonoBehaviour, IInteractable
{
    private Outline _outline;
    private Rigidbody _rb;
    private BoxCollider _col;

    [SerializeField] private ItemType itemType;
    [SerializeField] private GameObject puzzleCar;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _col = GetComponent<BoxCollider>();
        _outline = GetComponent<Outline>();

        puzzleCar.SetActive(false);
        _outline.enabled = false;
    }

    public void Interact()
    {
        if (Inventory.Instance.CurrentItem == null)
            return;

        if (Inventory.Instance.CurrentItem.ItemType !=  ItemType.Flashlight)
            return;

        puzzleCar.SetActive(true);
        Inventory.Instance.CurrentItem.Drop();
    }

    public void ShowOutline()
    {
        if (_outline != null)
            _outline.enabled = true;
    }

    public void HideOutline()
    {
        if (_outline != null)
            _outline.enabled = false;
    }
}
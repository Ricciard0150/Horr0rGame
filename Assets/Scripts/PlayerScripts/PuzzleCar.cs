using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Outline))]
public class PuzzleCar : MonoBehaviour, IInteractable
{
    private Outline _outline;
    private BoxCollider _col;

    [SerializeField] private ItemType itemType;
    [SerializeField] private GameObject puzzleCar;

    public int counter = 0;
    private void Start()
    {
        _col = GetComponent<BoxCollider>();
        _outline = GetComponent<Outline>();

        puzzleCar.SetActive(false);
        _outline.enabled = false;
    }

    public void Interact()
    {
        if (Inventory.Instance.CurrentItem == null)
            return;

        if (Inventory.Instance.CurrentItem.ItemType !=  ItemType.RightCarDoor)
            return;

        puzzleCar.SetActive(true);
        counter++;
        Destroy(Inventory.Instance.CurrentItem.gameObject);

        if(counter == 2)
        {
           print("Puzzle Car is complete!");
        }
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
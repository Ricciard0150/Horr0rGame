using UnityEngine;
using UnityEngine.SceneManagement;

public class UseCarToEscape : MonoBehaviour, IInteractable
{
    [SerializeField]private Collider _col;
    [SerializeField] private Outline outline;

    private void Start()
    {
        _col = GetComponent<Collider>();
        outline = GetComponent<Outline>();

        _col.enabled = false;

        if (outline != null)
            outline.enabled = false;
    }

    public void Interact()
    {
        if (Inventory.Instance.CurrentItem == null)
            return;

        if (Inventory.Instance.CurrentItem.ItemType != ItemType.KeyCar)
            return;

        SceneManager.LoadScene("VictoryScene");
    }

    public void ShowOutline()
    {
        if (GameController.Instance.CarPiecesPlaced == 2)
        {
            _col.enabled = true;

            if (outline != null)
                outline.enabled = true;
        }
    }

    public void HideOutline()
    {
        if (outline != null)
            outline.enabled = false;
    }
}
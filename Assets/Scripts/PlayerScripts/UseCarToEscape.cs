using UnityEngine;
using UnityEngine.SceneManagement;

public class UseCarToEscape : MonoBehaviour, IInteractable
{
    [SerializeField] private Collider _col;
    [SerializeField] private Outline outline;

    private bool canUse;

    private void Start()
    {
        _col = GetComponent<Collider>();
        outline = GetComponent<Outline>();

        _col.enabled = false;

        if (outline != null)
            outline.enabled = false;
    }

    private void Update()
    {
        if (!canUse && GameController.Instance.CarPiecesPlaced >= 3)
        {
            canUse = true;
            _col.enabled = true;
        }
    }

    public void Interact()
    {
        if (!canUse)
            return;

        if (Inventory.Instance.CurrentItem == null)
            return;

        if (Inventory.Instance.CurrentItem.ItemType != ItemType.KeyCar)
            return;

        StartCoroutine(ScreenFadeToVictory.Instance.FadeAndLoadScene("VictoryScene"));
    }

    public void ShowOutline()
    {
        if (canUse && outline != null)
            outline.enabled = true;
    }

    public void HideOutline()
    {
        if (outline != null)
            outline.enabled = false;
    }
}
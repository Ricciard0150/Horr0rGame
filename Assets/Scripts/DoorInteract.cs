using UnityEngine;

public class DoorInteract : MonoBehaviour, IInteractable
{
    [Header("Locker Points")]
    [SerializeField] private Transform hideSpot;
    [SerializeField] private Transform entering;
    [SerializeField] private Transform exitSpot;

    [Header("Player")]
    [SerializeField] private PlayerHiding player;

    private bool isInside;
    private Outline outline;

    void Start()
    {
        outline = GetComponent<Outline>();
        if (outline != null)
            outline.enabled = false;
    }

    public void Interact()
    {
        if (isInside)
        {
            ExitLocker();
        }
        else
        {
            EnterLocker();
        }
    }

    public void EnterLocker()
    {
        if (isInside) return;

        if (hideSpot == null || entering == null || exitSpot == null)
            return;
        

        player.MoveTo(hideSpot, entering, exitSpot, transform);
        isInside = true;
        
    }

    public void ExitLocker()
    {
        if (!isInside) return;

        player.ExitFrom();
        isInside = false;
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

    public bool IstnAvailable()
    {
        return !isInside;
    }
}
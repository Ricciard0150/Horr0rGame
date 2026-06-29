using UnityEngine;

public class DoorInteract : MonoBehaviour
{
    [Header("Pontos")]
    [SerializeField] private Transform hideSpot;
    [SerializeField] private Transform exitSpot;

    [Header("Player")]
    [SerializeField] private PlayerHiding player;

    private bool isInside;

    public void EnterLocker()
    {
        if (isInside) return;

        player.MoveTo(hideSpot);

        isInside = true;
    }

    public void ExitLocker()
    {
        if (!isInside) return;

        player.ExitFrom(exitSpot);

        isInside = false;
    }
}
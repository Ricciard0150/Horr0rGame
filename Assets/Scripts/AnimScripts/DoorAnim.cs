using System.Runtime.CompilerServices;
using UnityEngine;

public class DoorInteract : MonoBehaviour
{
    [Header("Movimento do Player")]
    public Transform walkTarget;
    public WalkToHide playerWalk;

    [SerializeField] private Transform _player;
    [SerializeField] private Transform _target;

    public void MovePlayerToDoor()
    {
        if (playerWalk != null && walkTarget != null)
        {
            _player.position = _target.position;
            playerWalk.StartWalkingTo(walkTarget);
        }
    }
}
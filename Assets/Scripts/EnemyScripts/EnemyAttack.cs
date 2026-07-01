using StarterAssets;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private PlayerLife playerLife;
    [SerializeField] private BloodScreen bloodScreen;
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<FirstPersonController>() != null)
        {
            playerLife.ReduceLife(1);
            bloodScreen.ShowBloodScreen();
            if (playerLife.GetCurrentLife() <= 0)
            {
                GameController.Instance.PlayerDie();
            }
            Debug.Log("Enemy attacked the player!");
        }
    }
}

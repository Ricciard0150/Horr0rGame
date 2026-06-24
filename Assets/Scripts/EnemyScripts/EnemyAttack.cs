using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private PlayerLife playerLife;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerLife.ReduceLife(1);
            if(playerLife.GetCurrentLife() <= 0)
            {
                GameController.Instance.PlayerDie();
            }
            Debug.Log("Enemy attacked the player!");
        }
    }
}

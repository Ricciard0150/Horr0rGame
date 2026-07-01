using StarterAssets;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private PlayerLife playerLife;
    [SerializeField] private BloodScreen bloodScreen;
    private bool hasAttacked = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<FirstPersonController>() != null && !hasAttacked)
        {
            playerLife.ReduceLife(1);
            bloodScreen.ShowBloodScreen();
            if (playerLife.GetCurrentLife() <= 0)
            {
                GameController.Instance.PlayerDie();
            }
            Debug.Log("Enemy attacked the player!");
            hasAttacked = true;
            StartCoroutine(ResetAttack());
        }
    }

    IEnumerator ResetAttack()
    {
        yield return new WaitForSeconds(0.8f); 
        hasAttacked = false;
    }
}

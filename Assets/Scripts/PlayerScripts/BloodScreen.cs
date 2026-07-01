using System.Collections;
using UnityEngine;

public enum BloodState
{
    None,
    FirstHit,
    SecondHit,
    thirdHit,
}
public class BloodScreen : MonoBehaviour
{
    [SerializeField] private PlayerLife playerLife;
    [SerializeField]private GameObject[] _bloodScreens;
    [SerializeField][Range(0.5f, 20)] private float lifeRegenerator;   
    private BloodState _currentState;
    void Start()
    {
        playerLife = GetComponent<PlayerLife>();        
    }
    public void ShowBloodScreen()
    {
        if (playerLife == null) 
            return;

        if(playerLife.GetCurrentLife() == 3)        
            SetState(BloodState.FirstHit);
        
        if(playerLife.GetCurrentLife() == 2)
            SetState(BloodState.SecondHit);

        if(playerLife.GetCurrentLife() == 1)
            SetState(BloodState.thirdHit);
    }

    public void SetState(BloodState newState)
    {
        BloodScreen bloodScreen = GetComponent<BloodScreen>();

        switch (_currentState)
        {
            case BloodState.None:                
                break;

            case BloodState.FirstHit:
                bloodScreen._bloodScreens[1].SetActive(false);
                StopAllCoroutines();
                break;

            case BloodState.SecondHit:
                bloodScreen._bloodScreens[2].SetActive(false);
                StopAllCoroutines();
                break;

            case BloodState.thirdHit:
                bloodScreen._bloodScreens[3].SetActive(false);                
                break;
        }
        _currentState = newState;
        switch (_currentState)
        { 
            case BloodState.None:
                bloodScreen._bloodScreens[bloodScreen._bloodScreens.Length - 1 - 2 - 3].SetActive(false); 
                StopAllCoroutines();
            break;

            case BloodState.FirstHit:
                bloodScreen._bloodScreens[1].SetActive(true);
                StartCoroutine(HealthRegenerator());
            break;

            case BloodState.SecondHit:
                bloodScreen._bloodScreens[2].SetActive(true);
                StartCoroutine(HealthRegenerator());
            break;

            case BloodState.thirdHit:
                bloodScreen._bloodScreens[3].SetActive (true);
                StartCoroutine(HealthRegenerator());
            break;
        }
    }

    IEnumerator HealthRegenerator()
    {
        if (playerLife.GetCurrentLife() <= playerLife._maxLife)
        {
            yield return new WaitForSeconds(lifeRegenerator);
            playerLife.GetRegen(1);
        }
    }
}

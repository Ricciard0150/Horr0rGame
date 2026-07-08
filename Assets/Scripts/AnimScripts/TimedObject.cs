using StarterAssets;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class TimedObject : MonoBehaviour
{
    [SerializeField] private GameObject targetObject;
    [SerializeField] private StarterAssetsInputs starterAssetsInputs;

    [SerializeField] private float timeToAppear = 45f;
    [SerializeField] private float activeTime = 10f;

    private bool coroutineStarted;   

    private void Start()
    {
        targetObject.SetActive(false);
    }

    private void Update()
    {
        if (!coroutineStarted &&
            (starterAssetsInputs.move.x != 0 || starterAssetsInputs.move.y != 0))
        {
            coroutineStarted = true;
            StartCoroutine(AppearRoutine());            
        }
    }

    private IEnumerator AppearRoutine()
    {
        
        while (true)
        {
            yield return new WaitForSeconds(timeToAppear);

            targetObject.SetActive(true);

            yield return new WaitForSeconds(activeTime);

            targetObject.SetActive(false);
        }
    }   
}
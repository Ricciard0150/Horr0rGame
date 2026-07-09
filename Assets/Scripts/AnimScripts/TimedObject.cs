using StarterAssets;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class TimedObject : MonoBehaviour
{
    [SerializeField] private GameObject targetObject;
    [SerializeField] private StarterAssetsInputs starterAssetsInputs;

    [SerializeField] private float timeToAppear = 0.5f;
    [SerializeField] private float activeTime = 0.5f;
   
    private bool coroutineStarted;
    [SerializeField] private AudioSource audioSourceWalk;
    private void Start()
    {
        targetObject.SetActive(false);
    }

    private void Update()
    {        
        if (!coroutineStarted && (starterAssetsInputs.move.x != 0 || starterAssetsInputs.move.y != 0))
        {
            coroutineStarted = true;
            StartCoroutine(AppearRoutine());
            audioSourceWalk.volume = 0.8f;
            audioSourceWalk.Play();
        }
        else if (coroutineStarted && starterAssetsInputs.move.x == 0 && starterAssetsInputs.move.y == 0)
        {
            coroutineStarted = false;
            StopCoroutine(AppearRoutine());
            targetObject.SetActive(false);
            audioSourceWalk.Stop();
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
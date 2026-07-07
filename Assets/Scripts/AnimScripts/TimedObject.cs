using System.Collections;
using UnityEngine;

public class TimedObject: MonoBehaviour
{
    [SerializeField] private GameObject targetObject;
    [SerializeField] private float timeToAppear = 45f;
    [SerializeField] private float activeTime = 10f;

    private void Start()
    {
        if (targetObject != null)
            targetObject.SetActive(false);

        StartCoroutine(AppearRoutine());
    }

    private IEnumerator AppearRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeToAppear);

            if (targetObject != null)
                targetObject.SetActive(true);

            yield return new WaitForSeconds(activeTime);

            if (targetObject != null)
                targetObject.SetActive(false);
        }
    }
}
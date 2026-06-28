using UnityEngine;
using System.Collections;

public class WalkToHide : MonoBehaviour
{
    public float waitTime = 3f;
    public float speed = 2f;

    private CharacterController characterController;
    private MonoBehaviour firstPersonController;
    private MonoBehaviour[] allScripts;
    private bool isWalking = false;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        firstPersonController = GetComponent<StarterAssets.FirstPersonController>();
        allScripts = GetComponents<MonoBehaviour>();
    }

    public void StartWalkingTo(Transform target)
    {
        if (isWalking || target == null) return;
        StartCoroutine(WaitAndWalk(target));
    }

    IEnumerator WaitAndWalk(Transform target)
    {
        isWalking = true;

        if (characterController != null)
            characterController.enabled = false;
        if (firstPersonController != null)
            firstPersonController.enabled = false;
        foreach (var script in allScripts)
        {
            if (script != this)
                script.enabled = false;
        }

        yield return new WaitForSeconds(waitTime);

        while (Vector3.Distance(transform.position, target.position) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                target.position,
                Time.deltaTime * speed
            );
            yield return null;
        }

        transform.position = target.position;

        if (characterController != null)
        {
            characterController.enabled = true;
            characterController.transform.position = target.position;
        }
        if (firstPersonController != null)
            firstPersonController.enabled = true;
        foreach (var script in allScripts)
        {
            if (script != this)
                script.enabled = true;
        }

        isWalking = false;
        Debug.Log($"Cheguei em: {target.name}");
    }
}
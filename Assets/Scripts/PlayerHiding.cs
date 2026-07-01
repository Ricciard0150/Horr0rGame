using UnityEngine;
using System.Collections;

public class PlayerHiding : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 2f;
    public float rotationSpeed = 180f;

    private CharacterController characterController;
    private MonoBehaviour firstPersonController;
    private bool isBusy;
    private Transform currentExitSpot;
    private Transform currentLocker;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        firstPersonController = GetComponent<StarterAssets.FirstPersonController>();
    }

    public void MoveTo(Transform hideSpot, Transform entering, Transform exitSpot, Transform locker)
    {
        if (isBusy || hideSpot == null || entering == null || exitSpot == null || locker == null) return;

        currentExitSpot = exitSpot;
        currentLocker = locker;
        StartCoroutine(MoveRoutine(hideSpot, entering, locker));
    }

    public void ExitFrom()
    {
        if (isBusy || currentExitSpot == null) return;

        StartCoroutine(ExitRoutine(currentExitSpot));
        currentExitSpot = null;
        currentLocker = null;
    }

    IEnumerator MoveRoutine(Transform hideSpot, Transform entering, Transform locker)
    {
        isBusy = true;
        DisablePlayer();

        transform.position = entering.position;

        Vector3 directionToLocker = (locker.position - transform.position).normalized;
        directionToLocker.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(directionToLocker);
        transform.rotation = targetRotation;

        while (Vector3.Distance(transform.position, hideSpot.position) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                hideSpot.position,
                Time.deltaTime * speed
            );
            yield return null;
        }

        transform.position = hideSpot.position;
        yield return Rotate180();

        isBusy = false;
    }

    IEnumerator ExitRoutine(Transform exitSpot)
    {
        isBusy = true;

        while (Vector3.Distance(transform.position, exitSpot.position) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                exitSpot.position,
                Time.deltaTime * speed
            );
            yield return null;
        }

        yield return Rotate180();
        transform.position = exitSpot.position;

        EnablePlayer();
        isBusy = false;
    }

    IEnumerator Rotate180()
    {
        Quaternion startRot = transform.rotation;
        Quaternion targetRot = startRot * Quaternion.Euler(0, 180f, 0);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * (rotationSpeed / 180f);
            transform.rotation = Quaternion.Slerp(startRot, targetRot, t);
            yield return null;
        }

        transform.rotation = targetRot;
    }

    void DisablePlayer()
    {
        if (characterController != null)
            characterController.enabled = false;

        if (firstPersonController != null)
            firstPersonController.enabled = false;
    }

    void EnablePlayer()
    {
        if (characterController != null)
            characterController.enabled = true;

        if (firstPersonController != null)
            firstPersonController.enabled = true;
    }

    public bool IsBusy() => isBusy;
}
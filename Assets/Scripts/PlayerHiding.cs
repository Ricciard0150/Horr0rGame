using UnityEngine;
using System.Collections;

public class PlayerHiding : MonoBehaviour
{
    public float speed = 2f;
    public float rotationSpeed = 180f;

    private CharacterController characterController;
    private MonoBehaviour firstPersonController;
    [SerializeField] private PlayerHiding ph;

    private bool isBusy;

    [SerializeField] private Transform player;
    [SerializeField] private Transform entering;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        firstPersonController = GetComponent<StarterAssets.FirstPersonController>();
    }

    public void MoveTo(Transform target)
    {
        player = GetComponent<CharacterController>().transform;

        if (isBusy || target == null) return;

        player.position = entering.position;
        StartCoroutine(MoveRoutine(target));
    }

    public void ExitFrom(Transform target)
    {
        if (isBusy || target == null) return;

        StartCoroutine(ExitRoutine(target));
    }

    IEnumerator MoveRoutine(Transform target)
    {
        isBusy = true;

        DisablePlayer();

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

        yield return Rotate180();

        isBusy = false;
    }

    IEnumerator ExitRoutine(Transform target)
    {
        isBusy = true;

        while (Vector3.Distance(transform.position, target.position) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                target.position,
                Time.deltaTime * speed
            );

            yield return null;
        }


        yield return Rotate180();

        transform.position = target.position;

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

            transform.rotation = Quaternion.Slerp(
                startRot,
                targetRot,
                t
            );

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
}
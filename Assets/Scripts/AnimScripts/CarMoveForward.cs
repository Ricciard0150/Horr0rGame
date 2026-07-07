using UnityEngine;

public class CarMoveForward : MonoBehaviour
{
    [SerializeField] private Transform targetPoint;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float stopDistance = 0.1f;

    void Update()
    {
        if (targetPoint == null)
            return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPoint.position,
            speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, targetPoint.position) <= stopDistance)
        {
            transform.position = targetPoint.position;
            enabled = false;
        }
    }
}
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PatrolController : MonoBehaviour
{
    [SerializeField] private Transform[] _patrolPoints; //array pq tem valor fixo
    private int _currentPointIndex;    
    [Header("Teleport near player")]
    [SerializeField] private Transform _playerPositions;
    private int closestIndex = 0;
    public Vector3 GetRandomPoint()
    {
          int randomIndex = Random.Range(0, _patrolPoints.Length);
        return _patrolPoints[randomIndex].localPosition;        
    }
    public Vector3 MoveToNextPoint()
    {
        if (_patrolPoints.Length == 0)
            return Vector3.zero;
        Vector3 nextPoint = _patrolPoints[_currentPointIndex].position;
        _currentPointIndex++;
        if(_currentPointIndex >= _patrolPoints.Length)
            _currentPointIndex = 0;         
        return nextPoint;
    }

    public Transform GetClosestPatrolPointIndex()
    {        
        float shortestDistance = float.MaxValue;

        for (int i = 0; i < _patrolPoints.Length; i++)
        {
            float distance = (_playerPositions.position - _patrolPoints[i].position).sqrMagnitude;

            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                closestIndex = i;
            }
        }

        return _patrolPoints[closestIndex];
    }    
}

using UnityEngine;

public class PatrolController : MonoBehaviour
{
    [SerializeField] private Transform[] _patrolPoints; //array pq tem valor fixo
    private int _currentPointIndex;    
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public Vector3 GetRandomPoint()
    {
          int randomIndex = Random.Range(0, _patrolPoints.Length);
        return _patrolPoints[randomIndex].position;
        //criar metodo para retornar o ponro de patrulha mais proximo do inimigo
        //criar metodo para retornar o ponto de patrulha mais proximo do player
    }
    public Vector3 MoveToNextPoint()
    {
               if (_patrolPoints.Length == 0)
            return Vector3.zero;
        Vector3 nextPoint = _patrolPoints[_currentPointIndex].localPosition;
        _currentPointIndex++;
        if(_currentPointIndex >= _patrolPoints.Length)
            _currentPointIndex = 0; 
        print ("next patrol point: " + nextPoint);
        return nextPoint;
    }
}

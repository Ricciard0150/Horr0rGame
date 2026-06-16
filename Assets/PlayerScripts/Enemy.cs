using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    Idle,
    Chasing,
    Patrolling
}
public class Enemy : MonoBehaviour
{
    private PatrolController _patrolController;
    private GameObject _nape;
    private NavMeshAgent _agent;
    private Transform _player;
    private EnemyState _currentState;
    [SerializeField][Range(0.5f, 5f)]private float _waitTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _nape = transform.GetChild(0).gameObject;
        _player = GameController.Instance.PlayerTransform;
        _patrolController = GameController.Instance.PatrolController;   
        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = 3f;

        SetState(EnemyState.Patrolling);
    }

    // Update is called once per frame
    void Update()
    {
        Vision();
        
    }
    public void Vision()
    {
        bool playerInSight = Physics.Linecast(transform.position, _player.position, out RaycastHit hit);
        if (playerInSight)
        {
            //aqui o enemy para
            if (!_currentState.Equals(EnemyState.Chasing))
                return;
            SetState(EnemyState.Idle);
        }
        else
        {
            if (_currentState.Equals(EnemyState.Chasing))
                return;
            StopAllCoroutines();
            SetState(EnemyState.Chasing);
        }
    }
    public void SetState(EnemyState newState)
    {
        Vector3 lastPlayerPos = _player.position;   
        switch (_currentState)
        {
            case EnemyState.Idle:
                StartCoroutine(Wait());
                break;
            case EnemyState.Chasing:
                _agent.SetDestination(lastPlayerPos);
                _nape.SetActive(false);
                break;
            case EnemyState.Patrolling:
                // Implement patrolling logic here
                break;
        }
        _currentState = newState;
        switch(_currentState)
        {
            case EnemyState.Idle:
                break;
            case EnemyState.Chasing:
                _nape.SetActive(false);
                _agent.SetDestination(_player.position);
                break;
            case EnemyState.Patrolling:
                // Implement patrolling logic here  
                print("inimigo começou a caça");
                _agent.SetDestination(_patrolController.MoveToNextPoint()); 
                StartCoroutine(Patrolling());
                break;
        }
    }
    IEnumerator Wait()
    {
        yield return new WaitUntil(() => _agent.remainingDistance <= _agent.stoppingDistance);
        yield return new WaitForSeconds(_waitTime + 3);
        SetState(EnemyState.Patrolling);
    }    
    IEnumerator Patrolling()
    {
        yield return new WaitUntil(() => _agent.remainingDistance <= _agent.stoppingDistance);
        SetState(EnemyState.Idle);
    }
} 

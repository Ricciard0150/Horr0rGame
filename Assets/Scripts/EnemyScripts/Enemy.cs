using StarterAssets;
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
    [Header("Patrol Controller")]
    [SerializeField] private PatrolController _patrolController;// Responsável por fornecer os pontos de patrulha para o inimigo, permitindo que ele se mova entre esses pontos quando estiver no estado de patrulha.
    [Space]
    [Header("NavMeshAgent")]
    private NavMeshAgent _agent;//Responsável por calcular rotas e mover o inimigo no ambiente usando a navegação do Unity.     
    [Space]
    [Header("PlayerInteractions")]
    [SerializeField] private BoxCollider _punchBoxCollider;  
    [SerializeField] private Transform _player;// Referência ao Transform do jogador, que é o alvo que o inimigo irá perseguir.  
    [Space]
    [Header("State")]
    private EnemyState _currentState = EnemyState.Idle;// Variável para armazenar o estado atual do inimigo, que pode ser Idle, Chasing ou Patrolling.
    [Space]
    [Header("WaitTime")]
    [SerializeField][Range(0.5f, 5)] private float _waitTime;
    [Space]
    [Header("Animator")]
    [SerializeField] private Animator animator;
    [Space]
    [Header("Audio")]
    [SerializeField] private GameObject attackAudioClip;
    [SerializeField] private GameObject ambientAudiosClip;
    IEnumerator Start()
    {
        _player = GameController.Instance.PlayerTransform; // Obtém a referência ao Transform do jogador a partir do GameController, que é um singleton responsável por gerenciar o jogo.
        _agent = GetComponent<NavMeshAgent>();
        //enemyPosition = _agent.transform.position.y; // Inicializa a posição do inimigo com a posição atual do GameObject.
        animator = GetComponentInChildren<Animator>();
        _punchBoxCollider.enabled = false;
        if (animator != null)
        {
            animator.SetBool("Spawn", true); // Define o parâmetro "Spawn" como true para iniciar a animação de spawn
            animator.SetBool("IsChasing", false);
            animator.SetBool("IsPatroling", false);
            animator.SetBool("IsIdle", false);
        }        
        yield return new WaitForSeconds(1f); // Espera 2 segundos para simular o tempo de spawn do inimigo, permitindo que a animação de spawn seja exibida antes de iniciar o comportamento do inimigo.
        animator.SetBool("Spawn", false); // Define o parâmetro "Spawn" como false para finalizar a animação de spawn        
        SetState(EnemyState.Patrolling);
    }   
    
    public void SetState(EnemyState newState)
    {
        //O primeiro swwitch é para simular um OnTriggerExit, onde o inimigo para de fazer algo relacionado ao estado anterior, e o segundo switch é para simular um OnTriggerEnter, onde o inimigo começa a fazer algo relacionado ao novo estado.
        Vector3 lastPlayerPosition = _player.position;// Armazena a última posição conhecida do jogador, que pode ser usada para o inimigo continuar perseguindo mesmo se perder a visão do jogador.
        switch (_currentState)
        {
            case EnemyState.Idle:               
                animator.SetBool("IsIdle", false);
                break;
            case EnemyState.Chasing:
                //_agent.SetDestination(_player.position);
                animator.SetBool("IsChasing", false);                
                break;
            case EnemyState.Patrolling:
                animator.SetBool("IsPatroling", false);
                StopCoroutine(TeleportPlayer());
                break;
        }
        _currentState = newState;// Atualiza o estado atual para o novo estado
        // O segundo switch é para simular um OnTriggerEnter, onde o inimigo começa a fazer algo relacionado ao novo estado.
        switch (_currentState)
        {
            case EnemyState.Idle:
                StartCoroutine(Wait());// Inicia a coroutine de espera para mudar para o estado de patrulha após um tempo.
                animator.SetBool("IsIdle", true); // Define o parâmetro "IsIdle" como false para finalizar a animação de idle
                animator.SetBool("IsPatroling", false);
                animator.SetBool("IsChasing", false);
                break;
            case EnemyState.Chasing:
                // _agent.isStopped = false; // Permite que o inimigo se mova               
                animator.SetBool("IsIdle", false);
                animator.SetBool("IsChasing", true);
                animator.SetBool("IsPatroling", false);                
                break;
            case EnemyState.Patrolling:               
                animator.SetBool("IsIdle", false);
                animator.SetBool("IsChasing", false);
                animator.SetBool("IsPatroling", true);
                _agent.SetDestination(_patrolController.MoveToNextPoint()); // Define o próximo ponto de patrulha para o inimigo.               
                StartCoroutine(Patrolling());     
                StartCoroutine(TeleportPlayer());
                break;
        }
    }
    IEnumerator Wait()
    {
        yield return new WaitForSeconds(_waitTime);
        SetState(EnemyState.Patrolling);
    }
    IEnumerator Patrolling()
    {
        yield return new WaitUntil(() => _agent.remainingDistance <= _agent.stoppingDistance); // Espera até que o inimigo chegue ao ponto de patrulha antes de definir o próximo ponto.        
        SetState(EnemyState.Idle);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<SoundObject>() != null)
        {
            StopCoroutine(Patrolling());
            StopCoroutine(Wait());
            SetState(EnemyState.Chasing);
            _agent.SetDestination(other.transform.position);            
        }       
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<FirstPersonController>() != null)
        {
            StopCoroutine(Patrolling());
            StopCoroutine(Wait());
            StartCoroutine(Attack());            
        }  
        if(other.GetComponent<SoundObject>() != null)
        {
            StopCoroutine(Patrolling());
            StopCoroutine(Wait());
            SetState(EnemyState.Chasing);
            _agent.SetDestination(other.transform.position);            
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<FirstPersonController>() != null)
        {
            StopCoroutine(Attack());
        }        
    }
    IEnumerator Attack()
    {        
        animator.SetBool("IsPunch", true);
        _punchBoxCollider.enabled = true;
        attackAudioClip.SetActive(true);
        ambientAudiosClip.SetActive(false);
        StopCoroutine(TeleportPlayer());
        yield return new WaitForSeconds(0.8f); 
        animator.SetBool("IsPunch", false);
        _punchBoxCollider.enabled = false;
        attackAudioClip.SetActive(false);
        ambientAudiosClip.SetActive(true);
        yield return new WaitForSeconds(5f); 
    }

    IEnumerator TeleportPlayer()
    {
        yield return new WaitForSeconds(30f);
        _agent.Warp(_patrolController.GetClosestPatrolPointIndex().position);
    }
}

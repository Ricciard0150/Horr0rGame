using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements.Experimental;

public enum EnemyState
{
    Idle,
    Chasing,
    Patrolling
}
public class Enemy : MonoBehaviour
{
    [SerializeField] private PatrolController _patrolController;// Responsável por fornecer os pontos de patrulha para o inimigo, permitindo que ele se mova entre esses pontos quando estiver no estado de patrulha.
    [SerializeField] private GameObject _nape;
    private NavMeshAgent _agent;//Responsável por calcular rotas e mover o inimigo no ambiente usando a navegação do Unity. 
    [SerializeField] private Transform _player;// Referência ao Transform do jogador, que é o alvo que o inimigo irá perseguir.
    //float _waitTime = 2f;// Tempo de espera para o inimigo mudar de estado, usado para simular um comportamento mais realista, como esperar um pouco antes de começar a perseguir o jogador.
    private EnemyState _currentState = EnemyState.Idle;// Variável para armazenar o estado atual do inimigo, que pode ser Idle, Chasing ou Patrolling.
    [SerializeField][Range(0.5f, 5)] private float _waitTime;
    [SerializeField] private Animator animator;

    IEnumerator Start()
    {
        _player = GameController.Instance.PlayerTransform; // Obtém a referência ao Transform do jogador a partir do GameController, que é um singleton responsável por gerenciar o jogo.
        _agent = GetComponent<NavMeshAgent>();

        //enemyPosition = _agent.transform.position.y; // Inicializa a posição do inimigo com a posição atual do GameObject.
        animator = GetComponentInChildren<Animator>();

        if (animator != null)
        {
            animator.SetBool("Spawn", true); // Define o parâmetro "Spawn" como true para iniciar a animação de spawn
            animator.SetBool("IsChasing", false);
            animator.SetBool("IsPatroling", false);
            animator.SetBool("IsIdle", false);
        }
        print(animator.name);
        yield return new WaitForSeconds(2f); // Espera 2 segundos para simular o tempo de spawn do inimigo, permitindo que a animação de spawn seja exibida antes de iniciar o comportamento do inimigo.
        animator.SetBool("Spawn", false); // Define o parâmetro "Spawn" como false para finalizar a animação de spawn
        print("chegou");
        SetState(EnemyState.Patrolling);
    }

    // Update is called once per frame
    void Update()
    {
        //Vision();       
    }
    public void Vision()
    {
       
        bool playerInSight = Physics.Linecast(transform.position, _player.position, out RaycastHit hit);
        if (playerInSight)
        { //no veo nadica de pyoer
            if (_currentState.Equals(EnemyState.Chasing))
                SetState(EnemyState.Idle);          
        }
        else
        {//Aqui o enemy persegue o jogador
            if (_currentState.Equals(EnemyState.Chasing))// Se já estiver no estado de perseguição, passa a ficar idle.
                return;
            StopAllCoroutines();// Para todas as coroutines em execução, como a de espera para mudar para o estado de patrulha, para garantir que o inimigo comece a perseguir imediatamente.
            SetState(EnemyState.Chasing);
        }        
    }
    public void SetState(EnemyState newState)
    {
        //O primeiro swwitch é para simular um OnTriggerExit, onde o inimigo para de fazer algo relacionado ao estado anterior, e o segundo switch é para simular um OnTriggerEnter, onde o inimigo começa a fazer algo relacionado ao novo estado.
        Vector3 lastPlayerPosition = _player.position;// Armazena a última posição conhecida do jogador, que pode ser usada para o inimigo continuar perseguindo mesmo se perder a visão do jogador.
        switch (_currentState)
        {
            case EnemyState.Idle:
                // Lógica para sair do estado Idle (a ser implementada)
                animator.SetBool("IsIdle", false);
                break;
            case EnemyState.Chasing:
                // Lógica para sair do estado Chasing (a ser implementada)
                _nape.SetActive(false);
                _agent.SetDestination(_player.position);
                animator.SetBool("IsChasing", false);
                break;
            case EnemyState.Patrolling:
                animator.SetBool("IsPatroling", false);
                print("PatrollingExit");
                // Lógica para sair do estado Patrolling (a ser implementada)
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
                _nape.SetActive(false);
                _agent.SetDestination(_player.position);
                animator.SetBool("IsIdle", false);
                animator.SetBool("IsChasing", true);
                animator.SetBool("IsPatroling", false);

                break;
            case EnemyState.Patrolling:
                // Lógica para patrulhar (a ser implementada)
                animator.SetBool("IsIdle", false);
                animator.SetBool("IsChasing", false);
                animator.SetBool("IsPatroling", true);
                _agent.SetDestination(_patrolController.MoveToNextPoint()); // Define o próximo ponto de patrulha para o inimigo.               
                StartCoroutine(Patrolling());
                print("PatrollingEnter");
                break;
        }
    }
    IEnumerator Wait()
    {
        Debug.LogError("Temporário");
        //yield return new WaitUntil(() => _agent.remainingDistance == _agent.stoppingDistance);        
        yield return new WaitForSeconds(_waitTime);  
        SetState(EnemyState.Patrolling);
    }
    IEnumerator Patrolling()
    {       
        yield return new WaitUntil(() => _agent.remainingDistance == _agent.stoppingDistance); // Espera até que o inimigo chegue ao ponto de patrulha antes de definir o próximo ponto.
        print("Chegou ao ponto de patrulha");
        SetState(EnemyState.Idle);      
    }
}

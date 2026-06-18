using UnityEngine;
using UnityEngine.Events;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; } //static pertence a classe, não a instância, ou seja, é compartilhada por todas as instâncias da classe. Já o get; private set; é uma propriedade que permite ler o valor de Instance de fora da classe, mas só permite atribuir um valor a Instance de dentro da classe. Isso é útil para garantir que apenas uma instância de GameController seja criada e acessível globalmente.
    public Transform PlayerTransform { get => _playerTransform; }
    public PatrolController PatrolController { get => _patrolController;}
    
    public PickUpItems HeldItem { get; set; }

    [Header("Scene References")]
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private PatrolController _patrolController;
    [SerializeField] private PickUpItems _pickUpItems;
    [Space]
    [Header("Events")]
    public UnityEvent OnUseBattery;
    public UnityEvent OnUseFlashlight;

    void Awake()
    {
        Instance = this;    
    }
}

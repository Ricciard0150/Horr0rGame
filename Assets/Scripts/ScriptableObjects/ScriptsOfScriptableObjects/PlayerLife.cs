using UnityEngine;

[CreateAssetMenu(fileName = "PlayerLife", menuName = "ScriptableObjects/PlayerLife", order = 1)]
public class PlayerLife : ScriptableObject
{
    private float _maxLife = 4;
    [SerializeField] private float _currentLife = 4;
    [SerializeField] private GameObject[] _bloodScreens;
    private void OnEnable()
    {
        _currentLife = 4; 
    }

    public void ReduceLife(float amount)
    {
        _currentLife -= amount;
        if (_currentLife < 0)
        {
            _currentLife = 0;
        }
    }

    public float GetCurrentLife()
    {
        return _currentLife;
    }
}
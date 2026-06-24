using UnityEngine;

[CreateAssetMenu(fileName = "PlayerLife", menuName = "ScriptableObjects/PlayerLife", order = 1)]
public class PlayerLife : ScriptableObject
{
    [SerializeField] private float _currentLife = 3;
    private void OnEnable()
    {
        _currentLife = 3; 
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
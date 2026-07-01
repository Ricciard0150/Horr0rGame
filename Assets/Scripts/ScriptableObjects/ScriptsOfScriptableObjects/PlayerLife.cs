using UnityEngine;

[CreateAssetMenu(fileName = "PlayerLife", menuName = "ScriptableObjects/PlayerLife", order = 1)]
public class PlayerLife : ScriptableObject
{
    public float _maxLife = 4;
    [SerializeField] private float _currentLife = 4;    
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

    public void GetRegen (float regen)
    {
        if (_currentLife >= _maxLife)
        {
            _currentLife = _maxLife;
            return;
        }       
        _currentLife += regen;       
    }
}
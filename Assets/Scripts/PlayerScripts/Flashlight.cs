using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public enum ActiveState
{
    OFF,
    ON
}

public class Flashlight : MonoBehaviour
{
    private ActiveState _activeState = ActiveState.OFF;
    private Light _lt;
    private float _originalIntensity;

    [SerializeField] private float _intensityDecreaseRate = 0.5f;
    [SerializeField] private Image[] batteryBars;
    [SerializeField] private float _batteryDuration = 1f;
    [SerializeField] private float _batteryTimer;

    private bool _lostingPower;

    // Propriedade para verificar se a bateria está cheia
    public bool IsBatteryFull => _batteryTimer >= _batteryDuration;

    void Start()
    {
        _lt = GetComponentInChildren<Light>();
        _originalIntensity = _lt.intensity;

        GameController.Instance.OnUseBattery.AddListener(Recharge);
        GameController.Instance.OnUseFlashlight.AddListener(TurnFlashlight);

        _batteryTimer = _batteryDuration;
        UpdateBatteryUI();
    }

    private void Recharge()
    {
        _lt.intensity = _originalIntensity;
        _batteryTimer = _batteryDuration;
        _lostingPower = false;

        UpdateBatteryUI();
    }

    void Update()
    {
        switch (_activeState)
        {
            case ActiveState.OFF:
                break;

            case ActiveState.ON:

                if (_lostingPower)
                {
                    if (_lt.intensity <= 0)
                        return;

                    _lt.intensity -= Time.deltaTime * _intensityDecreaseRate;
                }
                else
                {
                    _batteryTimer -= Time.deltaTime * _intensityDecreaseRate;

                    if (_batteryTimer <= 0)
                    {
                        _batteryTimer = 0;
                        _lostingPower = true;
                    }
                }

                break;
        }

        UpdateBatteryUI();
    }

    private void UpdateBatteryUI()
    {
        float percent = _batteryTimer / _batteryDuration;

        int activeBars;

        if (percent > 0.66f)
            activeBars = 3;
        else if (percent > 0.33f)
            activeBars = 2;
        else if (percent > 0f)
            activeBars = 1;
        else
            activeBars = 0;

        for (int i = 0; i < batteryBars.Length; i++)
        {
            batteryBars[i].enabled = i < activeBars;
        }
    }

    public void TurnFlashlight()
    {
        if (_activeState == ActiveState.ON)
            SetStateA(ActiveState.OFF);
        else
            SetStateA(ActiveState.ON);
    }

    public void SetStateA(ActiveState newState)
    {
        switch (newState)
        {
            case ActiveState.OFF:
                _lt.enabled = false;
                break;

            case ActiveState.ON:
                _lt.enabled = true;
                break;
        }

        _activeState = newState;
    }
}
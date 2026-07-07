using System.Collections;
using UnityEngine;
using UnityEngine.UI    ;

using UnityEngine.InputSystem;

public enum ActiveState
{
    OFF, ON //usa se qnd bool n é suficiente, ou seja, quando tem mais de 2 estados 
}
public class Flashlight : MonoBehaviour
{
    private ActiveState _activeState = ActiveState.OFF;
    private Light _lt;
    private float _originalIntensity;
    [SerializeField] private float _intensityDecreaseRate = 0.5f;
    [SerializeField] private Image[] batteryBars;
    [SerializeField] private float _batteryDuration = 1f;
    private bool _lostingPower;
    private bool _isFullBattery = true;
   [SerializeField] private float _batteryTimer;    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
        print("sdjhfgujgsnjnfdsifds");
        _lt.intensity = _originalIntensity;
        _batteryTimer = _batteryDuration;
        _lostingPower = false;

        UpdateBatteryUI();
    }
    // Update is called once per frame
    void Update()
    {
        /*
         * Esse switch e para simular algo como o ocollisionstay pois executa a 
         * logica a cada frame dependendo do estado atual da lanterna, ou seja, se estiver ligada, a cada frame ela vai perder intensidade, se estiver desligada, nao acontece nada
         */
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
                        _lostingPower = true;
                    }
                }
                break;
            default:
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
        /*
         * Com esse procedimento, semp0re que apertermos para ligar ou desligar a lanterna, iniciaremos um processo parecido
         * com OnCollisionEnter, onde podemos verificar alguma consequência dependendo do estado atual da lanterna que nao precisa ser executado a todo momento
         */
        if (_activeState.Equals(ActiveState.ON))
        {
            SetStateA(ActiveState.OFF);
        }
        else
        {
            SetStateA(ActiveState.ON);

        }
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
            default:
                break;
        }
        _activeState = newState;
    }
}

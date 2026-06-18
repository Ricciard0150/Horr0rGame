using UnityEngine;
using UnityEngine.Events;
[RequireComponent(typeof(Outline))]     
public class LightSwitch : MonoBehaviour, IInteractable
{
    [SerializeField] private bool isOn;
    [SerializeField] private UnityEvent OnTurnOn;
    [SerializeField] private UnityEvent OnTurnOff;

    private Outline outline;
    public void HideOutline()
    {
        if (outline != null)
        {
            outline.enabled = false;
        }
    }

    public void Interact()
    {
        if(isOn)
        {          
            OnTurnOff.Invoke();
            isOn = false;
        }
        else
        {
            OnTurnOn.Invoke();
            isOn = true;
        }

    }

    public void ShowOutline()
    {
        if (outline != null)
        {
            outline.enabled = true;
        }
    }
    private void Start()
    {
        outline = GetComponentInChildren<Outline>();
        outline.enabled = false;   
    }

}

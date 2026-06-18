using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Outline))]
public class PlankDoor : MonoBehaviour, IInteractable
{
    private Outline _outline; 
    bool isOpen = false;
    [SerializeField] 
    public void Interact()
    {

    }
    public void HideOutline()
    {
        if (_outline != null)
        {
            _outline.enabled = false;
        }
    }

    public void ShowOutline()
    {
        if (_outline != null)
        {
            _outline.enabled = true;
        }
    }

    void Start()
    {
        _outline = GetComponentInChildren<Outline>();
        _outline.enabled = false;
    }

}

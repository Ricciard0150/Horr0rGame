using UnityEngine;

[RequireComponent(typeof(Outline))]
public class Hide : MonoBehaviour, IInteractable
{
    private Outline _outline;
    private bool isHidden;

    public bool IsHidden => isHidden;

    [SerializeField] private Transform _hideSpot;
    [SerializeField] private Transform _exitSpot;
    [SerializeField] private Transform _player;

    void Start()
    {
        _outline = GetComponent<Outline>();
        _outline.enabled = false;
    }

    public void Interact()
    {
        print("a1");

        if (!isHidden)
        { 
            _player.position = _hideSpot.position;
            print("a2");
            isHidden = true;
        }
        else
        {
            _player.position = _exitSpot.position;
            isHidden = false;
        }
    }

    public void ShowOutline()
    {
        _outline.enabled = true;
    }

    public void HideOutline()
    {
        _outline.enabled = false;
    }
}
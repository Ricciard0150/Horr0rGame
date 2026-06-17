using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
public class Interaction : MonoBehaviour, IStatusPlayer
{

    [SerializeField] float _interactionRange = 3f;  
    private Camera _mainCam;
    private IInteractable _hit;
    public PickUpItems _pui;
    
    void Start()
    {
        _mainCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (!Physics.Raycast(_mainCam.transform.position, _mainCam.transform.forward, out RaycastHit hit, _interactionRange))
            return;

      
        if (hit.collider.TryGetComponent(out IInteractable interactable))
        {
            if (_hit == interactable)// SE FOR O MESMO OBJETO NAO FAÇA NADA 
                return;
            
            _hit?.HideOutline();
            _hit = interactable;
            _hit.ShowOutline();
        }

        else
        { // caso o raycast acerte algo que nai seja interagivel ou nao acerte nada, esconde o outline do objeto anterior   
            _hit?.HideOutline();
            _hit = null;
        }
    }

    public void OnInteract(InputValue value)
    {
        
        if (_hit == null)
            return;
        
        _hit.Interact();
    }
    public void OnDrop(InputValue value)
    {
        _pui.Drop();
    }
}

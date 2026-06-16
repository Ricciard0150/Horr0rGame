using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
public class Inventory : MonoBehaviour
{
    [SerializeField] float _interactionRange = 3f;
    private Camera _mainCam;
    private ICollectable _hit;
    [SerializeField]private int _batteries;

    void Start()
    {
        _mainCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (!Physics.Raycast(_mainCam.transform.position, _mainCam.transform.forward, out RaycastHit hit, _interactionRange))
            return;
        if (hit.collider.TryGetComponent(out ICollectable collecta))
        {
            if (_hit == collecta)// SE FOR O MESMO OBJETO NAO FAÇA NADA 
                return;

            _hit?.HideOutline();
            _hit = collecta;
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

        _hit.Collect();
        _batteries++;   
    }
    public void OnRechange(InputValue value)
    {
        if (_batteries <= 0)
            return;

        _batteries--;
        GameController.Instance.OnUseBattery.Invoke();
    }

    public void OnUseFlashlight(InputValue value)
    {
        GameController.Instance.OnUseFlashlight.Invoke();
    }
}


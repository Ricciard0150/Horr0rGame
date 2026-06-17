using UnityEngine;
using UnityEngine.InputSystem;

public class Inventory : MonoBehaviour
{
    [SerializeField] private float _interactionRange = 3f;
    [SerializeField] private Transform _holdPoint;

    private Camera _mainCam;
    private ICollectable _hit;

    [SerializeField] private int _batteries;

    private void Start()
    {
        _mainCam = Camera.main;
    }

    private void Update()
    {
        if (!Physics.Raycast(
                _mainCam.transform.position,
                _mainCam.transform.forward,
                out RaycastHit hit,
                _interactionRange))
        {
            _hit?.HideOutline();
            _hit = null;
            return;
        }

        if (hit.collider.TryGetComponent(out ICollectable collectable))
        {
            if (_hit == collectable)
                return;

            _hit?.HideOutline();

            _hit = collectable;
            _hit.ShowOutline();
        }
        else
        {
            _hit?.HideOutline();
            _hit = null;
        }
    }

    public void OnInteract(InputValue value)
    {
        if (_hit == null)
            return;

        _hit.Collect(_holdPoint);

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
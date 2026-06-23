using UnityEngine;
using UnityEngine.InputSystem;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;

    [SerializeField] private float _interactionRange = 3f;
    [SerializeField] private Transform _holdPoint;

    private Camera _mainCam;
    private ICollectable _hit;

    [SerializeField] private int _batteries;

    private PickUpItems _currentItem;

    public PickUpItems CurrentItem => _currentItem;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _mainCam = Camera.main;
    }

    private void Update()
    {
        if (!Physics.Raycast(_mainCam.transform.position, _mainCam.transform.forward,
            out RaycastHit hit, _interactionRange))
        {
            ClearHit();
            return;
        }

        if (hit.collider.TryGetComponent(out ICollectable collectable))
        {
            if (_hit == collectable)
                return;

            ClearHit();

            _hit = collectable;
            _hit.ShowOutline();
        }
        else
        {
            ClearHit();
        }
    }

    private void ClearHit()
    {
        if (_hit != null)
            _hit.HideOutline();

        _hit = null;
    }

    public void SetItem(PickUpItems item)
    {
        _currentItem = item;
    }

    public void ClearItem()
    {
        _currentItem = null;
    }

    public void OnInteract(InputValue value)
    {
        if (_hit == null)
            return;

        _hit.Collect(_holdPoint);
    }

    public void OnRechange(InputValue value)
    {
        if (_batteries <= 0)
            return;

        _batteries--;
        GameController.Instance.OnUseBattery.Invoke();
    }

    public void OnDrop(InputValue value)
    {
        _currentItem?.Drop();
    }

    public void OnUseFlashlight(InputValue value)
    {
        GameController.Instance.OnUseFlashlight.Invoke();
    }
}
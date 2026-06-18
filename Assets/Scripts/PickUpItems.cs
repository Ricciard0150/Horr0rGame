using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Outline))]
public class PickUpItems : MonoBehaviour, ICollectable
{
    private Rigidbody rb;
    private BoxCollider boxCollider;
    private Outline outline;

    private bool isCollected;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        outline = GetComponent<Outline>();
        outline.enabled = false;
    }

    public void Collect(Transform parent)
    {
        if (isCollected)
            return;

        isCollected = true;

        transform.SetParent(parent);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        rb.isKinematic = true;
        boxCollider.enabled = false;

        outline.enabled = false;
    }
    public void Drop()
    {
        if (!isCollected)
            return;
        isCollected = false;
        transform.SetParent(null);
        rb.isKinematic = false;
        boxCollider.enabled = true;
    }

    public void ShowOutline()
    {
        if (!isCollected)
            outline.enabled = true;
    }

    public void HideOutline()
    {
        outline.enabled = false;
    }
}
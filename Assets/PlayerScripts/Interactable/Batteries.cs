using UnityEngine;
[RequireComponent(typeof(Outline))]
public class Batteries : MonoBehaviour, ICollectable
{
    private Outline outline;
    public void Collect(Transform parent)
    {
        Destroy(gameObject);
    }

    public void Drop()
    {
       //nothing;
    }

    public void HideOutline()
    {
        if (outline != null)
        {
            outline.enabled = false;
        }
    }

    public void ShowOutline()
    {
        if (outline != null)
        {
            outline.enabled = true;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        outline = GetComponentInChildren<Outline>();
        outline.enabled = false;
    }
}

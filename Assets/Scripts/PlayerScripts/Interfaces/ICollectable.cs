using UnityEngine;

public interface ICollectable
{
    void Collect(Transform parent);
    void Drop();
    void ShowOutline();
    void HideOutline(); 
}
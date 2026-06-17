using UnityEngine;

public interface ICollectable
{
    void Collect(Transform parent);
    void ShowOutline();
    void HideOutline();
}
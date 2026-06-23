using UnityEngine;

public interface ICollectable
{
    ItemType ItemType { get; }

    void Collect(Transform parent);
    void Drop();
    void ShowOutline();
    void HideOutline();
}
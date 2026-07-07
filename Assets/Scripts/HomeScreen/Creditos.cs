using UnityEngine;

public class Creditos : MonoBehaviour
{
    public float velocidade = 40f;

    RectTransform rect;

    void Start()
    {
        rect = GetComponent<RectTransform>();
    }

    void Update()
    {
        rect.anchoredPosition += Vector2.up * velocidade * Time.deltaTime;
    }
}
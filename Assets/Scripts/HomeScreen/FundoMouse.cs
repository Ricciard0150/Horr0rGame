using UnityEngine;

public class FundoMouse : MonoBehaviour
{
    public float intensidade = 20f;
    public float suavidade = 5f;

    private RectTransform rectTransform;
    private Vector2 posicaoInicial;
    private Vector2 alvo;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        posicaoInicial = rectTransform.anchoredPosition;
    }

    void Update()
    {
        Vector2 mouse = Input.mousePosition;

        float x = (mouse.x / Screen.width - 0.5f) * intensidade;
        float y = (mouse.y / Screen.height - 0.5f) * intensidade;

        alvo = posicaoInicial + new Vector2(-x, -y);

        rectTransform.anchoredPosition = Vector2.Lerp(
            rectTransform.anchoredPosition,
            alvo,
            Time.deltaTime * suavidade
        );
    }
}
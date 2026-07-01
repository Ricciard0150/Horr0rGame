using UnityEngine;
using UnityEngine.SceneManagement;

public class TelaInicial : MonoBehaviour
{
    [Header("Painéis")]
    public GameObject painelConfiguracoes;
    public GameObject painelCreditos;


    public void IniciarJogo()
    {
        SceneManager.LoadScene("NomeDaCena");
    }

    public void AbrirConfiguracoes()
    {
        painelConfiguracoes.SetActive(true);
    }

    public void FecharConfiguracoes()
    {
        painelConfiguracoes.SetActive(false);
    }

    public void AbrirCreditos()
    {
        painelCreditos.SetActive(true);
    }

    public void FecharCreditos()
    {
        painelCreditos.SetActive(false);
    }

    public void SairJogo()
    {
        Debug.Log("Saindo do jogo...");

        Application.Quit();
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Painéis")]
    public GameObject painelConfiguracoes;
    public GameObject painelCreditos;

    [Header("Cena do Jogo")]
    public string nomeDaCena;

    private void Start()
    {
        painelConfiguracoes.SetActive(false);
        painelCreditos.SetActive(false);
    }

    public void IniciarJogo()
    {
        SceneManager.LoadScene(nomeDaCena);
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
    public void SairDoJogo()
    {
        Debug.Log("Saindo do jogo...");

        Application.Quit();
    }
}
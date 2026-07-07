using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    [Header("Painéis")]
    public GameObject painelConfiguracoes;
    public GameObject painelCreditos;

    [Header("Botão Fechar Configurações")]
    public Image imagemFecharConfiguracoes;
    public Sprite spriteNormalConfiguracoes;
    public Sprite spriteVermelhoConfiguracoes;

    [Header("Botão Fechar Créditos")]
    public Image imagemFecharCreditos;
    public Sprite spriteNormalCreditos;
    public Sprite spriteVermelhoCreditos;

    public float tempoAnimacao = 0.15f;

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
        StartCoroutine(AnimacaoFecharConfiguracoes());
    }

    IEnumerator AnimacaoFecharConfiguracoes()
    {
        imagemFecharConfiguracoes.sprite = spriteVermelhoConfiguracoes;

        yield return new WaitForSeconds(tempoAnimacao);

        imagemFecharConfiguracoes.sprite = spriteNormalConfiguracoes;
        painelConfiguracoes.SetActive(false);
    }

    public void AbrirCreditos()
    {
        painelCreditos.SetActive(true);
    }

    public void FecharCreditos()
    {
        StartCoroutine(AnimacaoFecharCreditos());
    }

    IEnumerator AnimacaoFecharCreditos()
    {
        imagemFecharCreditos.sprite = spriteVermelhoCreditos;

        yield return new WaitForSeconds(tempoAnimacao);

        imagemFecharCreditos.sprite = spriteNormalCreditos;
        painelCreditos.SetActive(false);
    }
    public void SairDoJogo()
    {
        Debug.Log("Saindo do jogo...");

        Application.Quit();
    }
}
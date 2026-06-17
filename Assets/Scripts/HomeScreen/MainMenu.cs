using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Iniciar jogo
    public void ComeçarJogo()
    {
        SceneManager.LoadScene("Game");
    }

    // Abrir painel de configurações
    public void AbrirConfiguracoes(GameObject settingsPanel)
    {
        settingsPanel.SetActive(true);
    }

    // Fechar painel de configurações
    public void FecharConfiguracoes(GameObject settingsPanel)
    {
        settingsPanel.SetActive(false);
    }

    // Sair do jogo
    public void SairJogo()
    {
        Debug.Log("Saindo do jogo...");

        Application.Quit();
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Iniciar jogo
    public void ComeçarJogo()
    {
        SceneManager.LoadScene("Game");
    }
    [SerializeField] private GameObject settingsPanel;

    public void AbrirConfiguracoes()
    {
        settingsPanel.SetActive(true);
    }

    public void FecharConfiguracoes()
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
using UnityEngine;

public class PlayerQuit : MonoBehaviour
{
    void Update()
    {
        // Verifica se o jogador apertou a tecla ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Fecha o jogo
            QuitGames();
        }
    }
    void QuitGames()
    {
#if UNITY_EDITOR
        // Se estiver rodando no editor, apenas para o modo de play
        UnityEditor.EditorApplication.isPlaying = false;
#else
            // Se for buildado (jogo real), fecha o aplicativo
            Application.Quit();
#endif
    }
}

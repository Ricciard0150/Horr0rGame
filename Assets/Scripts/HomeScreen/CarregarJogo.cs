using UnityEngine;
using UnityEngine.SceneManagement;

public class CarregarJogo : MonoBehaviour
{
    private void Start()
    {
        SceneManager.LoadScene("Jogo", LoadSceneMode.Additive);
    }
}
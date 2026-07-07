using UnityEngine;
using UnityEngine.UI;

public class TelaCheia : MonoBehaviour
{
    public Toggle toggleTelaCheia;

    void Start()
    {
        toggleTelaCheia.isOn = Screen.fullScreen;
        Debug.Log("TelaCheia iniciado. Estado atual: " + Screen.fullScreen);
    }

    public void AlterarTelaCheia()
    {
        Screen.fullScreen = toggleTelaCheia.isOn;
        Debug.Log("Tela cheia alterada para: " + Screen.fullScreen);
    }
}
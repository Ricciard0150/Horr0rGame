using UnityEngine;
using UnityEngine.UI;

public class TelaCheia : MonoBehaviour
{
    public Toggle toggleTelaCheia;

    void Start()
    {
        toggleTelaCheia.isOn = Screen.fullScreen;
    }

    public void AlterarTelaCheia()
    {
        Screen.fullScreen = toggleTelaCheia.isOn;
    }
}
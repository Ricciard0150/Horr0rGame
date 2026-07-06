using UnityEngine;
using UnityEngine.UI;

public class ControleTelaCheia : MonoBehaviour
{
    public Toggle toggleTelaCheia;

    void Start()
    {
        toggleTelaCheia.isOn = Screen.fullScreen;

        toggleTelaCheia.onValueChanged.AddListener(AlterarTelaCheia);
    }

    public void AlterarTelaCheia(bool telaCheia)
    {
        Screen.fullScreen = telaCheia;
    }
}
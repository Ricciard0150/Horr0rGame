using UnityEngine;
using UnityEngine.UI;

public class VolumeGeral : MonoBehaviour
{
    public Slider sliderVolume;
    public Toggle toggleMudo;
    public Image iconeToggle;

    public Sprite iconeSom;
    public Sprite iconeMudo;

    void Start()
    {
        sliderVolume.value = AudioListener.volume;
        AtualizarToggle();
    }

    public void AlterarVolume()
    {
        AudioListener.volume = sliderVolume.value;

        AtualizarToggle();
    }

    public void MudarMudo()
    {
        if (toggleMudo.isOn)
        {
            AudioListener.volume = 0;
            sliderVolume.value = 0;
        }
        else
        {
            AudioListener.volume = 1;
            sliderVolume.value = 1;
        }

        AtualizarToggle();
    }

    void AtualizarToggle()
    {
        if (AudioListener.volume == 0)
        {
            iconeToggle.sprite = iconeMudo;
            toggleMudo.isOn = true;
        }
        else
        {
            iconeToggle.sprite = iconeSom;
            toggleMudo.isOn = false;
        }
    }
}
using UnityEngine;
using UnityEngine.UI;

public class VolumeGeral : MonoBehaviour
{
    public Slider sliderVolume;
    public Toggle toggleMudo;

    public Image iconeToggle;
    public Sprite iconeSom;
    public Sprite iconeMudo;

    private float ultimoVolume = 1f;

    void Start()
    {
        sliderVolume.value = AudioListener.volume;
        AtualizarInterface();
    }

    public void AlterarVolume()
    {
        AudioListener.volume = sliderVolume.value;

        if (sliderVolume.value > 0)
            ultimoVolume = sliderVolume.value;

        AtualizarInterface();
    }

    public void MudarMudo(bool mudo)
    {
        if (mudo)
        {
            ultimoVolume = sliderVolume.value;

            AudioListener.volume = 0;
            sliderVolume.SetValueWithoutNotify(0);
        }
        else
        {
            AudioListener.volume = ultimoVolume;
            sliderVolume.SetValueWithoutNotify(ultimoVolume);
        }

        AtualizarInterface();
    }

    void AtualizarInterface()
    {
        bool mudo = AudioListener.volume <= 0.001f;

        toggleMudo.SetIsOnWithoutNotify(mudo);

        iconeToggle.sprite = mudo ? iconeMudo : iconeSom;
    }
}
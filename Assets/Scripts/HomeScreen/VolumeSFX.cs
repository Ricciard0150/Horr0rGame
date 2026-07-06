using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSFX : MonoBehaviour
{
    public AudioMixer mixer;

    public Slider sliderVolume;
    public Toggle toggleMudo;
    public Image iconeToggle;

    public Sprite iconeSom;
    public Sprite iconeMudo;

    public void AlterarVolume()
    {
        float volume = sliderVolume.value;

        mixer.SetFloat("VolumeSFX", Mathf.Log10(volume) * 20);

        AtualizarIcone(volume);
    }

    public void MudarMudo()
    {
        if (toggleMudo.isOn)
        {
            sliderVolume.value = 0.0001f;
        }
        else
        {
            sliderVolume.value = 1;
        }

        AlterarVolume();
    }

    void AtualizarIcone(float volume)
    {
        if (volume <= 0.0001f)
            iconeToggle.sprite = iconeMudo;
        else
            iconeToggle.sprite = iconeSom;
    }
}
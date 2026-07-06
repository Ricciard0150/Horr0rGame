using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class ControleVolumeSFX : MonoBehaviour
{
    [Header("Áudio")]
    public AudioMixer mixer;

    [Header("UI")]
    public Slider sliderVolume;
    public Toggle toggleMudo;
    public Image imagemIcone;

    [Header("Sprites")]
    public Sprite iconeSom;
    public Sprite iconeMudo;

    private float ultimoVolume = 1f;

    void Start()
    {
        sliderVolume.onValueChanged.AddListener(AlterarVolume);
        toggleMudo.onValueChanged.AddListener(AlternarMudo);

        AlterarVolume(sliderVolume.value);
    }

    public void AlterarVolume(float volume)
    {
        if (!toggleMudo.isOn)
        {
            ultimoVolume = volume;

            float volumeDB = Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20;
            mixer.SetFloat("VolumeSFX", volumeDB);
        }
    }

    public void AlternarMudo(bool mudo)
    {
        if (mudo)
        {
            mixer.SetFloat("VolumeSFX", -80f);
            imagemIcone.sprite = iconeMudo;
        }
        else
        {
            AlterarVolume(ultimoVolume);
            imagemIcone.sprite = iconeSom;
        }
    }
}
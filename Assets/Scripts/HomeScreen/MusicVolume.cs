using UnityEngine;
using UnityEngine.UI;

public class MusicVolume : MonoBehaviour
{
    public AudioSource musicSource;
    public Slider musicSlider;

    private void Start()
    {
        float volume = PlayerPrefs.GetFloat("MusicVolume", 1f);

        musicSlider.value = volume;
        musicSource.volume = volume;

        musicSlider.onValueChanged.AddListener(ChangeVolume);
    }

    public void ChangeVolume(float volume)
    {
        musicSource.volume = volume;
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }
}
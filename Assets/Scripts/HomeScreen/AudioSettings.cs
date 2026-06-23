using UnityEngine;
using UnityEngine.Audio;

public class AudioSettings : MonoBehaviour
{
    public AudioMixer mixer;

    public void SetMasterVolume(float volume)
    {
        volume = Mathf.Clamp(volume, 0.0001f, 1f);

        mixer.SetFloat(
            "MasterVolume",
            Mathf.Log10(volume) * 20
        );
    }

    public void SetMusicVolume(float volume)
    {
        volume = Mathf.Clamp(volume, 0.0001f, 1f);

        mixer.SetFloat(
            "MusicVolume",
            Mathf.Log10(volume) * 20
        );
    }
}
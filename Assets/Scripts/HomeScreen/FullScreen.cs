using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreenMode = isFullscreen
            ? FullScreenMode.FullScreenWindow
            : FullScreenMode.Windowed;

            Debug.Log("O botão foi clicado! Valor: " + isFullscreen);
    }

    public void SetMasterVolume(float volume)
    {
        AudioListener.volume = volume;
    }

    public void SetMusicVolume(float volume)
    {
        // Futuro
    }

    public void SetSFXVolume(float volume)
    {
        // Futuro
    }
}
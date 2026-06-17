using UnityEngine;
using UnityEngine.Video;

public class TV : MonoBehaviour, IInteractable
{
    private VideoPlayer videoPlayer;
    private GameObject videoContainer;
    private Outline outline;

    public void HideOutline()
    {
        if (outline != null)
        {
            outline.enabled = false;
        }
    }

    public void Interact()
    {
       if (videoPlayer.isPlaying)
        {
            videoPlayer.Stop();
            videoContainer.SetActive(false);
        }
        else
        {
                videoPlayer.Play();
                videoContainer.SetActive(true);
        }   
    }

    public void ShowOutline()
    {
        if (outline != null)
        {
            outline.enabled = true;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        videoContainer = transform.GetChild(0).gameObject;
        outline = GetComponentInChildren<Outline>();
        outline.enabled = false;
    }
}

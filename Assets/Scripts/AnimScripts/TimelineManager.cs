using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

public class TextFadeIn : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textUI;
    [SerializeField] private float fadeDuration = 2f;
    [SerializeField] private PlayableDirector director;


    private void Start()
    {
        StartCoroutine(FadeText());   
            director.Play();
    }

    IEnumerator FadeText()
    {
        Color color = textUI.color;
        color.a = 0;
        textUI.color = color;

        float time = 0;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;

            float alpha = Mathf.Lerp(0, 1, time / fadeDuration);

            color.a = alpha;
            textUI.color = color;

            yield return null;
        }

        color.a = 1;
        textUI.color = color;
    }
}
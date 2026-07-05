using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class ScreenFadeToVictory : MonoBehaviour
{
    public static ScreenFadeToVictory Instance;

    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            transform.SetParent(null);

            DontDestroyOnLoad(gameObject);

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Color color = fadeImage.color;
        color.a = 0;
        fadeImage.color = color;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(DelayedFadeOut());
    }

    private IEnumerator DelayedFadeOut()
    {
        yield return null;
        yield return null;

        StartCoroutine(FadeOut());
    }
    public IEnumerator FadeIn()
    {
        float time = 0;

        Color color = fadeImage.color;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;

            color.a = Mathf.Lerp(0, 1, time / fadeDuration);
            fadeImage.color = color;

            yield return null;
        }

        color.a = 1;
        fadeImage.color = color;
    }

    public IEnumerator FadeOut()
    {
        float time = 0;

        Color color = fadeImage.color;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;

            color.a = Mathf.Lerp(1, 0, time / fadeDuration);
            fadeImage.color = color;

            yield return null;
        }

        color.a = 0;
        fadeImage.color = color;
    }

    public IEnumerator FadeAndLoadScene(string sceneName)
    {

        yield return StartCoroutine(FadeIn());

        SceneManager.LoadScene(sceneName);

        yield return null;

  
        yield return StartCoroutine(FadeOut());
    }
}
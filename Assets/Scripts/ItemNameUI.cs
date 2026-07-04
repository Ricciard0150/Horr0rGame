using System.Collections;
using TMPro;
using UnityEngine;

public class ItemNameUI : MonoBehaviour
{
    public static ItemNameUI Instance;

    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private float visibleTime = 1.5f;
    [SerializeField] private float fadeTime = 1f;

    Coroutine routine;

    private void Awake()
    {
        Instance = this;

        Color c = text.color;
        c.a = 0;
        text.color = c;
    }

    public void Show(string itemName)
    {
        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(ShowRoutine(itemName));
    }

    IEnumerator ShowRoutine(string itemName)
    {
        text.text = itemName;

        Color c = text.color;
        c.a = 1;
        text.color = c;

        yield return new WaitForSeconds(visibleTime);

        float t = 0;

        while (t < fadeTime)
        {
            t += Time.deltaTime;

            c.a = Mathf.Lerp(1, 0, t / fadeTime);
            text.color = c;

            yield return null;
        }

        c.a = 0;
        text.color = c;
    }
}
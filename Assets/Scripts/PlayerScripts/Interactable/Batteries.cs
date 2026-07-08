using System.Collections;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Outline))]
public class Batteries : MonoBehaviour, ICollectable
{
    private Outline outline;

    [SerializeField] private Flashlight flashlight;

    [Header("UI")]
    [SerializeField] private TMP_Text warningText;
    [SerializeField] private float showTime = 1.5f;
    [SerializeField] private float fadeTime = 1f;

    public ItemType ItemType => ItemType.Battery;

    public void Collect(Transform parent)
    {
        if (flashlight.IsBatteryFull)
        {
            StopAllCoroutines();
            StartCoroutine(ShowWarning("you have enough"));
            return;
        }

        GameController.Instance.OnUseBattery.Invoke();
        Destroy(gameObject);
    }

    private IEnumerator ShowWarning(string message)
    {
        warningText.text = message;
        warningText.gameObject.SetActive(true);

   
        Color color = warningText.color;
        color.a = 1f;
        warningText.color = color;

        yield return new WaitForSeconds(showTime);

       
        float timer = 0f;

        while (timer < fadeTime)
        {
            timer += Time.deltaTime;

            color.a = Mathf.Lerp(1f, 0f, timer / fadeTime);
            warningText.color = color;

            yield return null;
        }

        color.a = 0f;
        warningText.color = color;
        warningText.gameObject.SetActive(false);
    }

    public void Drop()
    {
        throw new System.NotImplementedException();
    }

    public void HideOutline()
    {
        if (outline != null)
            outline.enabled = false;
    }

    public void ShowOutline()
    {
        if (outline != null)
            outline.enabled = true;
    }

    private void Start()
    {
        outline = GetComponentInChildren<Outline>();
        outline.enabled = false;

        if (warningText != null)
        {
            Color color = warningText.color;
            color.a = 0f;
            warningText.color = color;
            warningText.gameObject.SetActive(false);
        }
    }
}
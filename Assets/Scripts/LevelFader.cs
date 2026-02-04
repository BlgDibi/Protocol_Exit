using UnityEngine;
using System.Collections;
using UnityEngine.UI; 

public class LevelFader : MonoBehaviour
{
    [Header("Settings")]
    public CanvasGroup fadeGroup; 
    public float fadeDuration = 5f;

    void Start()
    {
        if (fadeGroup != null)
        {
            fadeGroup.alpha = 1;
            StartCoroutine(FadeInRoutine());
        }
    }

    IEnumerator FadeInRoutine()
    {
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            yield return null;
        }

        fadeGroup.alpha = 0;
        gameObject.SetActive(false);
    }
}
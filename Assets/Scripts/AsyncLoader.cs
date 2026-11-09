using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class AsyncLoader : MonoBehaviour
{
    [Header("UI Settings Loading Bar, Progress text")]
    public Slider loadingBar;
    public TextMeshProUGUI progressText;

    public static int TargetSceneIndex = -1;
    public static bool IsLoadingFromSave = false;

    private void Start()
    {
        StartCoroutine(LoadLevelAsync());
    }

    IEnumerator LoadLevelAsync()
    {
        int sceneToLoad;

        if (TargetSceneIndex != -1)
        {
            sceneToLoad = TargetSceneIndex;
        }
        else
        {
            sceneToLoad = 0;
        }

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad);
        operation.allowSceneActivation = false;

        float visualProgress = 0f;

        while (operation.isDone == false)
        {
            float targetProgress = Mathf.Clamp01(operation.progress / 0.9f);
            visualProgress = Mathf.MoveTowards(visualProgress, targetProgress, Time.deltaTime * 0.7f);

            if (loadingBar != null)
            {
                loadingBar.value = visualProgress;
            }

            if (progressText != null)
            {
                progressText.text = $"Loading... {Mathf.RoundToInt(visualProgress * 100)}%";
            }

            if (visualProgress >= 0.99f && operation.progress >= 0.9f)
            {
                if (loadingBar != null)
                {
                    loadingBar.value = 1f;
                }

                if (progressText != null)
                {
                    progressText.text = "Loading... 100%";
                }

                yield return new WaitForSeconds(0.3f);

                if (IsLoadingFromSave == true)
                {
                    SaveSystem.IsGameJustLoaded = true;
                    IsLoadingFromSave = false;
                }

                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
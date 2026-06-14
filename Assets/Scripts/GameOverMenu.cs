using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameOverMenu : MonoBehaviour
{
    public GameObject gameoverui;
    public GameObject youDiedText;
    public GameObject buttonPanel;

    [Tooltip("Delay before buttons")]
    public float delayBeforeButtons = 2f;
    public GameObject crosshair;

    private CanvasGroup youDiedCanvasGroup;
    private CanvasGroup buttonPanelCanvasGroup;

    private void Start()
    {
        if (youDiedText != null) youDiedText.SetActive(true);
        if (buttonPanel != null) buttonPanel.SetActive(true);

        if (youDiedText != null) youDiedCanvasGroup = youDiedText.GetComponent<CanvasGroup>();
        if (buttonPanel != null) buttonPanelCanvasGroup = buttonPanel.GetComponent<CanvasGroup>();

        if (youDiedCanvasGroup != null) youDiedCanvasGroup.alpha = 0;
        if (buttonPanelCanvasGroup != null) buttonPanelCanvasGroup.alpha = 0;

        if (buttonPanel != null) buttonPanel.SetActive(false);
        if (gameoverui != null) gameoverui.SetActive(false);
    }

    public void gameover()
    {
        if (gameoverui != null) gameoverui.SetActive(true);

        if (crosshair != null) crosshair.SetActive(false);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        StartCoroutine(GameOverSequence());
    }

    private IEnumerator GameOverSequence()
    {
        if (youDiedText != null) youDiedText.SetActive(true);
        yield return new WaitForSecondsRealtime(1f);

        if (youDiedCanvasGroup != null)
            yield return StartCoroutine(FadeInCanvasGroup(youDiedCanvasGroup, 1f));

        yield return new WaitForSecondsRealtime(delayBeforeButtons);

        if (youDiedCanvasGroup != null)
            yield return StartCoroutine(FadeOutCanvasGroup(youDiedCanvasGroup));

        if (youDiedText != null) youDiedText.SetActive(false);

        if (buttonPanel != null)
        {
            buttonPanel.SetActive(true);
            if (buttonPanelCanvasGroup != null)
                yield return StartCoroutine(FadeInCanvasGroup(buttonPanelCanvasGroup, 1f));
        }
    }

    private IEnumerator FadeInCanvasGroup(CanvasGroup canvasGroup, float targetAlpha)
    {
        float timeElapsed = 0f;
        float startingAlpha = canvasGroup.alpha;

        while (timeElapsed < 1f)
        {
            canvasGroup.alpha = Mathf.Lerp(startingAlpha, targetAlpha, timeElapsed);
            timeElapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        canvasGroup.alpha = targetAlpha;
    }

    private IEnumerator FadeOutCanvasGroup(CanvasGroup canvasGroup)
    {
        float timeElapsed = 0f;
        float startingAlpha = canvasGroup.alpha;

        while (timeElapsed < 1f)
        {
            canvasGroup.alpha = Mathf.Lerp(startingAlpha, 0f, timeElapsed);
            timeElapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0f;
    }

    public void restart()
    {
        Time.timeScale = 1f;

        if (gameoverui != null) gameoverui.SetActive(false);
        if (buttonPanel != null) buttonPanel.SetActive(false);

        if (youDiedCanvasGroup != null) youDiedCanvasGroup.alpha = 0f;
        if (buttonPanelCanvasGroup != null) buttonPanelCanvasGroup.alpha = 0f;

        if (KeyInventory.Instance != null) KeyInventory.Instance.ResetKeys();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            HP hp = player.GetComponent<HP>();
            if (hp != null) hp.Hp = 100;
            player.transform.position = Vector3.zero;
            player.transform.rotation = Quaternion.identity;
        }

        if (crosshair != null) crosshair.SetActive(true);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;

        if (gameoverui != null) gameoverui.SetActive(false);
        if (buttonPanel != null) buttonPanel.SetActive(false);
        if (youDiedCanvasGroup != null) youDiedCanvasGroup.alpha = 0f;
        if (buttonPanelCanvasGroup != null) buttonPanelCanvasGroup.alpha = 0f;

        SceneManager.LoadScene("MENU");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
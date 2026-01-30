using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameOverMenu : MonoBehaviour
{
    public GameObject gameoverui;
    public GameObject youDiedText;
    public GameObject buttonPanel;
    public float delayBeforeButtons = 5f;
    public GameObject crosshair;

    private CanvasGroup youDiedCanvasGroup;
    private CanvasGroup buttonPanelCanvasGroup;


    private void Start()
    {
        youDiedCanvasGroup = youDiedText.GetComponent<CanvasGroup>();
        buttonPanelCanvasGroup = buttonPanel.GetComponent<CanvasGroup>();
        youDiedCanvasGroup.alpha = 0;
        buttonPanelCanvasGroup.alpha = 0;
        gameoverui.SetActive(false);
    }

    public void gameover()
    {
        gameoverui.SetActive(true);

        if (crosshair != null)
        {
            crosshair.SetActive(false);
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        StartCoroutine(GameOverSequence());
    }

    private IEnumerator GameOverSequence()
    {
        yield return new WaitForSeconds(2f); 
        yield return StartCoroutine(FadeInCanvasGroup(youDiedCanvasGroup, 1f));
        yield return new WaitForSeconds(delayBeforeButtons);
        yield return StartCoroutine(FadeOutCanvasGroup(youDiedCanvasGroup));
        yield return StartCoroutine(FadeInCanvasGroup(buttonPanelCanvasGroup, 1f));
    }

    private IEnumerator FadeInCanvasGroup(CanvasGroup canvasGroup, float targetAlpha)
    {
        float timeElapsed = 0f;
        float startingAlpha = canvasGroup.alpha;

        while (timeElapsed < 1f)
        {
            canvasGroup.alpha = Mathf.Lerp(startingAlpha, targetAlpha, timeElapsed);
            timeElapsed += Time.deltaTime;
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
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 0f;
    }

    public void restart()
    {
        if (KeyInventory.Instance != null)
            KeyInventory.Instance.ResetKeys();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            HP hp = player.GetComponent<HP>();
            if (hp != null)
                hp.Hp = 100; 
            player.transform.position = Vector3.zero;
            player.transform.rotation = Quaternion.identity;
        }

        if (crosshair != null)
            crosshair.SetActive(true);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MENU");
    }

    public void Quit()
    {
        Application.Quit();
    }
}

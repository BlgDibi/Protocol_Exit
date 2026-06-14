using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class Ending1 : MonoBehaviour
{
    [Header("UI References")]
    public GameObject congratulationsUI;
    public GameObject buttonPanel;
    public string nextScene = "MENU";

    [Header("Camera")]
    public Camera mainCamera;
    private bool isCameraLocked = false;
    private CanvasGroup buttonPanelCanvasGroup;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip goodchoiceClip;

    private bool hasTriggered = false;

    void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (buttonPanel != null)
        {
            buttonPanelCanvasGroup = buttonPanel.GetComponent<CanvasGroup>();
            if (buttonPanelCanvasGroup != null)
                buttonPanelCanvasGroup.alpha = 0;

            buttonPanel.SetActive(false);
        }

        if (congratulationsUI != null)
            congratulationsUI.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered) 
        {
            hasTriggered = true; 
            Debug.Log("Player entered Ending trigger!");

            other.gameObject.SetActive(false);

            TriggerGameEnd();
        }
    }

    private void TriggerGameEnd()
    {
        if (congratulationsUI != null)
            congratulationsUI.SetActive(true);

        LockCamera(true);
        EnableCursor(true);

        if (buttonPanel != null)
            StartCoroutine(ShowButtonsWithDelay());
    }

    private IEnumerator ShowButtonsWithDelay()
    {
        if (audioSource != null && goodchoiceClip != null)
        {
            PlaySound(goodchoiceClip);
            yield return new WaitForSeconds(goodchoiceClip.length);
        }

        yield return new WaitForSeconds(2f);

        buttonPanel.SetActive(true);

        if (buttonPanelCanvasGroup != null)
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

    private void LockCamera(bool isLocked)
    {
        isCameraLocked = isLocked;
       
    }

    private void EnableCursor(bool enable)
    {
        Cursor.lockState = enable ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = enable;
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(nextScene);
    }

    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}

using UnityEngine;
using System.Collections;
using TMPro;

public class Ending3 : MonoBehaviour
{
    [Header("Teleport Settings")]
    public Transform teleportTarget;
    private bool used = false;

    [Header("UI Settings")]
    public GameObject gameOverUI;
    public GameObject buttonPanel;
    public GameObject playerModel;
    public GameObject gravestonePrefab;
    public float delayBeforeButtons = 1f;

    private CanvasGroup buttonPanelCanvasGroup;
    private bool deathTriggered = false;

    private void Awake()
    {
        if (buttonPanel != null)
        {
            buttonPanelCanvasGroup = buttonPanel.GetComponent<CanvasGroup>();
            if (buttonPanelCanvasGroup != null)
                buttonPanelCanvasGroup.alpha = 0;
            buttonPanel.SetActive(false);
        }

        if (gameOverUI != null)
            gameOverUI.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (used) return;

        if (other.CompareTag("Player"))
        {
            used = true;
            StartCoroutine(TeleportPlayer(other.gameObject));
        }
    }

    private IEnumerator TeleportPlayer(GameObject player)
    {
        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null)
            cc.enabled = false;

        if (teleportTarget != null)
        {
            player.transform.position = teleportTarget.position;
            player.transform.rotation = teleportTarget.rotation;
        }

        yield return null;

        if (cc != null)
            cc.enabled = true;
    }

    public void TriggerSteamDeath()
    {
        if (deathTriggered) return;
        deathTriggered = true;

        if (!gameObject.activeInHierarchy)
            gameObject.SetActive(true);

        StartCoroutine(SteamDeathSequence());
    }

    private IEnumerator SteamDeathSequence()
    {
        if (playerModel != null)
            playerModel.SetActive(false);
        if (gravestonePrefab != null)
            Instantiate(gravestonePrefab, playerModel.transform.position, Quaternion.identity);

        if (gameOverUI != null)
            gameOverUI.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        yield return new WaitForSeconds(delayBeforeButtons);

        TMP_Text tmp = gameOverUI.GetComponentInChildren<TMP_Text>();
        if (tmp != null)
            tmp.enabled = false;

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
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
    }
}

using UnityEngine;
using System.Collections;
using TMPro;
using StarterAssets;

public class Book : MonoBehaviour
{
    [Header("Dialogue")]
    public AudioClip introClip;
    [TextArea(3, 10)]
    public string introText;
    public AudioSource audioSource;

    [Header("UI")]
    public TextMeshProUGUI subtitleText;
    public GameObject promptCanvas;

    [Header("Player")]
    public ThirdPersonController playerController;
    public StarterAssetsInputs playerInputs;

    private bool playerInRange = false;
    private bool hasPlayed = false;
    public bool HasPlayed => hasPlayed;

    void Update()
    {
        if (playerInRange && !hasPlayed && Input.GetKeyDown(KeyCode.F))
        {
            StartIntro();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasPlayed)
        {
            if (promptCanvas != null)
                promptCanvas.SetActive(true);
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (promptCanvas != null)
                promptCanvas.SetActive(false);
            playerInRange = false;
        }
    }

    public void StartIntro()
    {
        if (playerController != null)
        {
            playerController.freezeAll = true;
        }

        if (ConditionManager.Instance != null)
        {
            ConditionManager.Instance.SetCondition("BookRead", true);
        }
        else
        {
        
        }

        if (hasPlayed) return;

        hasPlayed = true;

        if (playerController != null) playerController.canMove = false;
        if (playerInputs != null)
        {
            playerInputs.cursorInputForLook = false;
            playerInputs.move = Vector2.zero;
            playerInputs.look = Vector2.zero;

            playerInputs.enabled = false;
        }

        if (promptCanvas != null) promptCanvas.SetActive(false);

        if (subtitleText != null)
        {
            subtitleText.text = introText;
            subtitleText.gameObject.SetActive(true);
        }

        float duration = 3f;
        if (introClip != null && audioSource != null)
        {
            audioSource.clip = introClip;
            audioSource.Play();
            duration = introClip.length;
        }

        StartCoroutine(HideTextAfterDelay(duration));
    }

    private IEnumerator HideTextAfterDelay(float duration)
    {
        yield return new WaitForSeconds(duration);

        if (subtitleText != null)
            subtitleText.gameObject.SetActive(false);

        if (playerController != null) playerController.canMove = true;
        if (playerInputs != null) playerInputs.enabled = true; 
        if (playerInputs != null) playerInputs.cursorInputForLook = true;
        if (playerController != null)
        {
            playerController.freezeAll = false;
        }

    }

}

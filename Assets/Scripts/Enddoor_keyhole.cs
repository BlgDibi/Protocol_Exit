using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Localization;

public class Enddoor_keyhole : MonoBehaviour, InterfaceInteractable
{
    [Header("Key Settings")]
    [Tooltip("What kind of key does it need")]
    public Key requiredKey;

    [Tooltip("Condition for the lock")]
    public string conditionToSet;

    [Header("Visuals")]
    [Tooltip("3D Key which is in the lock")]
    public GameObject insertedKeyVisual;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip insertSound;
    public AudioClip errorSound;

    [Header("UI Message")]
    public GameObject messagePanel;
    public TextMeshProUGUI messageText;
    public float messageDuration = 3f;

    [Header("Localization")]
    public LocalizedString missingKeyMessage;
    public LocalizedString interactPrompt;

    private bool isSolved = false;
    [SerializeField] private KeyCode interactionKey = KeyCode.F;

    void Start()
    {
        if (insertedKeyVisual != null) insertedKeyVisual.SetActive(false);
        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        if (!string.IsNullOrEmpty(conditionToSet))
        {
            bool isSolvedInSaveOrMemory = SaveSystem.IsPuzzleSolved(conditionToSet) || (ConditionManager.Instance != null && ConditionManager.Instance.CheckCondition(conditionToSet));

            if (isSolvedInSaveOrMemory)
            {
                isSolved = true;
                if (insertedKeyVisual != null) insertedKeyVisual.SetActive(true);
                if (ConditionManager.Instance != null) ConditionManager.Instance.SetCondition(conditionToSet, true);
            }
            else
            {
                isSolved = false;
                if (insertedKeyVisual != null) insertedKeyVisual.SetActive(false);
                if (ConditionManager.Instance != null) ConditionManager.Instance.SetCondition(conditionToSet, false);
            }
        }
    }

    public void Interact()
    {
        if (isSolved) return;

        if (requiredKey != null && KeyInventory.Instance.HasKey(requiredKey))
        {
            KeyInventory.Instance.RemoveKey(requiredKey);

            if (insertedKeyVisual != null) insertedKeyVisual.SetActive(true);
            isSolved = true;

            if (audioSource != null && insertSound != null) audioSource.PlayOneShot(insertSound);

            if (!string.IsNullOrEmpty(conditionToSet))
            {
                ConditionManager.Instance.SetCondition(conditionToSet, true);
                SaveSystem.SavePuzzleSolved(conditionToSet);
            }

            if (messagePanel != null) messagePanel.SetActive(false);
        }
        else
        {
            if (audioSource != null && errorSound != null) audioSource.PlayOneShot(errorSound);
            ShowTemporaryMessage(missingKeyMessage.IsEmpty ? "I need a specific key for this lock." : missingKeyMessage.GetLocalizedString());
        }
    }

    public string GetInteractText()
    {
        if (isSolved) return "";
        return interactPrompt.IsEmpty ? "Insert Key" : interactPrompt.GetLocalizedString();
    }

    public KeyCode GetInteractKey()
    {
        return interactionKey;
    }

    private void ShowTemporaryMessage(string text)
    {
        if (messagePanel != null)
        {
            messageText.text = text;
            messagePanel.SetActive(true);
            StopAllCoroutines();
            StartCoroutine(HideMessageAfterDelay());
        }
    }

    private IEnumerator HideMessageAfterDelay()
    {
        yield return new WaitForSeconds(messageDuration);
        if (messagePanel != null) messagePanel.SetActive(false);
    }
}
using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Localization;

public class UniversalDoor : MonoBehaviour, InterfaceInteractable
{
    [Header("Movement Settings")]
    [SerializeField] private Transform doorPivot;
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float closeAngle = 0f;
    [SerializeField] private float rotationSpeed = 2f;

    [Header("Lock")]
    public bool isLocked = false;

    [Tooltip("Key")]
    public Key requiredKey;

    [Tooltip("Conditions")]
    public string requiredCondition;

    [Header("Sounds")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip closeSound;
    [SerializeField] private AudioClip lockedSound;

    [Header("UI Message")]
    [Tooltip("Doormessage Canvas")]
    public GameObject messagePanel;
    [Tooltip("Text")]
    public TextMeshProUGUI messageText;
    [Tooltip("Time to see the text")]
    public float messageDuration = 3f;

    [Header("Localization Keys")]
    [Tooltip("Text")]
    public LocalizedString lockedMessageKey;

    [Header("Localization Text")]
    public LocalizedString textOpenDoor;
    public LocalizedString textCloseDoor;


    private bool isDoorOpen = false;
    private bool isAnimating = false;
    private Quaternion targetRotation;

    private void Start()
    {
        targetRotation = Quaternion.Euler(0f, closeAngle, 0f);
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (messagePanel != null)
            messagePanel.SetActive(false);
    }

    private void Update()
    {
        if (isAnimating)
        {
            doorPivot.localRotation = Quaternion.Lerp(doorPivot.localRotation, targetRotation, Time.deltaTime * rotationSpeed);
            if (Quaternion.Angle(doorPivot.localRotation, targetRotation) < 0.1f)
            {
                doorPivot.localRotation = targetRotation;
                isAnimating = false;
            }
        }
    }

    public void Interact()
    {


        if (!isLocked)
        {
            ToggleDoor();
            return;
        }

        if (!string.IsNullOrEmpty(requiredCondition))
        {
            if (ConditionManager.Instance.CheckCondition(requiredCondition))
            {
                UnlockAndOpen();
                if (messagePanel != null) messagePanel.SetActive(false);
            }
            else
            {
                HandleLockedDoor();
            }
            return;
        }

        if (requiredKey != null)
        {
            if (KeyInventory.Instance.HasKey(requiredKey))
            {
                KeyInventory.Instance.RemoveKey(requiredKey);
                UnlockAndOpen();
                if (messagePanel != null) messagePanel.SetActive(false);
            }
            else
            {
                HandleLockedDoor(true);
            }
            return;
        }

        HandleLockedDoor();
    }

    public string GetInteractText()
    {
        

        if (isDoorOpen)
        {
            return textCloseDoor.GetLocalizedString();
        }
        else
        {
            return textOpenDoor.GetLocalizedString();
        }
    }

    public void KeypadSuccess()
    {
        if (isLocked || !isDoorOpen)
        {
            UnlockAndOpen();
            if (messagePanel != null) messagePanel.SetActive(false);
        }
    }

    private void HandleLockedDoor(bool isKeyMissing = false)
    {
        PlaySound(lockedSound);

        if (isKeyMissing && requiredKey != null)
        {
            ShowTemporaryMessage($"Locked!");
            return;
        }

        if (!lockedMessageKey.IsEmpty)
        {
            ShowTemporaryMessage(lockedMessageKey.GetLocalizedString());
        }
        else
        {
            if (!string.IsNullOrEmpty(requiredCondition)) Debug.Log($"Closed! The '{requiredCondition}' condition is missing.");
            ShowTemporaryMessage("Locked. I need to find something first.");
        }
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
        if (messageText != null) messageText.gameObject.SetActive(true);
    }

    private IEnumerator HideMessageAfterDelay()
    {
        yield return new WaitForSeconds(messageDuration);
        if (messagePanel != null) messagePanel.SetActive(false);
    }

    private void UnlockAndOpen()
    {
        isLocked = false;
        ToggleDoor();
    }

    private void ToggleDoor()
    {
        isDoorOpen = !isDoorOpen;
        targetRotation = Quaternion.Euler(0f, isDoorOpen ? openAngle : closeAngle, 0f);
        isAnimating = true;
        PlaySound(isDoorOpen ? openSound : closeSound);
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null) audioSource.PlayOneShot(clip);
    }

    public KeyCode GetInteractKey()
    {
        return KeyCode.E;
    }

}
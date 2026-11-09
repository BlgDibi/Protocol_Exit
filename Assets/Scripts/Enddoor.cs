using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Localization;

public class Enddoor : MonoBehaviour, InterfaceInteractable
{
    [Header("Movement Settings")]
    [Tooltip("Double door")]
    [SerializeField] private Transform leftDoorPivot;
    [SerializeField] private Transform rightDoorPivot;

    [SerializeField] private float leftOpenAngle = 90f;
    [SerializeField] private float rightOpenAngle = -90f;
    [SerializeField] private float closeAngle = 0f;
    [SerializeField] private float rotationSpeed = 2f;

    [Header("Lock Settings")]
    public bool isLocked = false;

    [Tooltip("Required Conditions ")]
    public List<string> requiredConditions = new List<string>();

    [Header("Sounds")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip closeSound;
    [SerializeField] private AudioClip lockedSound;

    [Header("UI Message")]
    public GameObject messagePanel;
    public TextMeshProUGUI messageText;
    public float messageDuration = 3f;

    [Header("Localization Keys")]
    public LocalizedString lockedMessageKey;
    public LocalizedString textOpenDoor;
    public LocalizedString textCloseDoor;

    [SerializeField] private KeyCode interactionKey = KeyCode.F;

    private bool isDoorOpen = false;
    private bool isAnimating = false;

    private bool hasSavedTime = false;

    private Quaternion targetRotationLeft;
    private Quaternion targetRotationRight;

    private void Start()
    {
        targetRotationLeft = Quaternion.Euler(0f, closeAngle, 0f);
        targetRotationRight = Quaternion.Euler(0f, closeAngle, 0f);

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (messagePanel != null)
        {
            messagePanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (isAnimating == true)
        {
            bool leftDone = true;
            bool rightDone = true;

            if (leftDoorPivot != null)
            {
                leftDoorPivot.localRotation = Quaternion.Lerp(leftDoorPivot.localRotation, targetRotationLeft, Time.deltaTime * rotationSpeed);

                if (Quaternion.Angle(leftDoorPivot.localRotation, targetRotationLeft) < 0.1f)
                {
                    leftDoorPivot.localRotation = targetRotationLeft;
                }
                else
                {
                    leftDone = false;
                }
            }

            if (rightDoorPivot != null)
            {
                rightDoorPivot.localRotation = Quaternion.Lerp(rightDoorPivot.localRotation, targetRotationRight, Time.deltaTime * rotationSpeed);

                if (Quaternion.Angle(rightDoorPivot.localRotation, targetRotationRight) < 0.1f)
                {
                    rightDoorPivot.localRotation = targetRotationRight;
                }
                else
                {
                    rightDone = false;
                }
            }

            if (leftDone == true && rightDone == true)
            {
                isAnimating = false;
            }
        }
    }

    public void Interact()
    {
        if (isLocked == false)
        {
            ToggleDoor();
            return;
        }

        bool canOpen = true;

        if (requiredConditions != null && requiredConditions.Count > 0)
        {
            foreach (string cond in requiredConditions)
            {
                if (string.IsNullOrEmpty(cond) == false && ConditionManager.Instance.CheckCondition(cond) == false)
                {
                    canOpen = false;
                    break;
                }
            }
        }

        if (canOpen == true)
        {
            UnlockAndOpen();

            if (messagePanel != null)
            {
                messagePanel.SetActive(false);
            }
        }
        else
        {
            HandleLockedDoor();
        }
    }

    public string GetInteractText()
    {
        if (isDoorOpen == true)
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
        if (isLocked == true)
        {
            isLocked = false;

            if (messagePanel != null)
            {
                messagePanel.SetActive(false);
            }
        }
    }

    private void HandleLockedDoor()
    {
        PlaySound(lockedSound);

        if (lockedMessageKey.IsEmpty == false)
        {
            ShowTemporaryMessage(lockedMessageKey.GetLocalizedString());
        }
        else
        {
            ShowTemporaryMessage("Locked. I need to unlock all mechanisms first.");
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

        if (messageText != null)
        {
            messageText.gameObject.SetActive(true);
        }
    }

    private IEnumerator HideMessageAfterDelay()
    {
        yield return new WaitForSeconds(messageDuration);

        if (messagePanel != null)
        {
            messagePanel.SetActive(false);
        }
    }

    private void UnlockAndOpen()
    {
        isLocked = false;
        ToggleDoor();
    }

    private void ToggleDoor()
    {
        if (isDoorOpen == true)
        {
            isDoorOpen = false;
        }
        else
        {
            isDoorOpen = true;
        }

        if (isDoorOpen == true)
        {
            targetRotationLeft = Quaternion.Euler(0f, leftOpenAngle, 0f);
            targetRotationRight = Quaternion.Euler(0f, rightOpenAngle, 0f);
            PlaySound(openSound);
        }
        else
        {
            targetRotationLeft = Quaternion.Euler(0f, closeAngle, 0f);
            targetRotationRight = Quaternion.Euler(0f, closeAngle, 0f);
            PlaySound(closeSound);
        }

        isAnimating = true;

        if (isDoorOpen == true && hasSavedTime == false)
        {
            Stopwatch stopwatch = FindFirstObjectByType<Stopwatch>();

            if (stopwatch != null)
            {
                stopwatch.StopStopwatch();
                stopwatch.SaveTimeOnMainMenu();

                int currentGameIndex = PlayerPrefs.GetInt("GameCount", 1) - 1;
                PlayerPrefs.SetInt("GameCompleted" + currentGameIndex, 1);
                PlayerPrefs.Save();

                float currentTime = stopwatch.GetElapsedTime();
                float bestTime = PlayerPrefs.GetFloat("BestEscapeTime", float.MaxValue);

                if (currentTime < bestTime)
                {
                    PlayerPrefs.SetFloat("BestEscapeTime", currentTime);
                    PlayerPrefs.Save();
                }
            }

            hasSavedTime = true;
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    public KeyCode GetInteractKey()
    {
        return interactionKey;
    }
}
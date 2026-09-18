using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Localization;

public class UniversalDoor : MonoBehaviour, InterfaceInteractable
{
    [Header("Movement Settings")]
    [SerializeField] private Transform doorPivot;
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float closeAngle = 0f;
    [SerializeField] private float rotationSpeed = 2f;

    [Header("Lock & Planks Protection")]
    public bool isLocked = false;
    public Key requiredKey;
    public string requiredCondition;

    [Header("Protecting Planks ")]
    public List<string> protectingPlankIDs;

    [Header("Sounds")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip closeSound;
    [SerializeField] private AudioClip lockedSound;

    [Header("UI Message")]
    public GameObject messagePanel;
    public TextMeshProUGUI messageText;
    public float messageDuration = 3f;

    [Header("Localization")]
    public LocalizedString lockedMessageKey;
    public LocalizedString textOpenDoor;
    public LocalizedString textCloseDoor;

    [Header("Save System")]
    public string doorID;
    [SerializeField] private KeyCode interactionKey = KeyCode.F;

    private bool isDoorOpen = false;
    private bool isAnimating = false;
    private Quaternion targetRotation;

    private IEnumerator Start()
    {
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        if (messagePanel != null) messagePanel.SetActive(false);

        yield return new WaitForSeconds(0.1f);

        string rewardState;

        if (DialogueManager.Instance != null)
        {
            rewardState = DialogueManager.Instance.GetVariableState("got_reward");
        }
        else
        {
          
            rewardState = "";
        }

        bool wasUnlockedBySave = !string.IsNullOrEmpty(doorID) && SaveSystem.IsDoorUnlocked(doorID);

        bool planksCleared = ProtectingPlanksRemoved();

        if (rewardState.ToLower() == "true" || wasUnlockedBySave || planksCleared)
        {
            isLocked = false;
        }

        if (rewardState.ToLower() == "true" || wasUnlockedBySave)
        {
            isLocked = false;
            isDoorOpen = true;
            doorPivot.localRotation = Quaternion.Euler(0f, openAngle, 0f);
            targetRotation = doorPivot.localRotation;
        }
        else
        {
            targetRotation = Quaternion.Euler(0f, closeAngle, 0f);
        }
    }

    private void Update()
    {
        if (isAnimating && doorPivot != null)
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
        if (isLocked && ProtectingPlanksRemoved())
        {
            UnlockAndOpen();
            return;
        }

        if (HasProtectingPlanks())
        {
            PlaySound(lockedSound);
            ShowTemporaryMessage("The door is blocked by wooden boards.");
            return;
        }

        if (!isLocked)
        {
            ToggleDoor();
            return;
        }

        if (requiredKey != null)
        {
            if (KeyInventory.Instance.HasKey(requiredKey))
            {
                KeyInventory.Instance.RemoveKey(requiredKey);
                UnlockAndOpen();
                return;
            }
            else 
            { 
                HandleLockedDoor(true); 
                return; 
            }
        }

        if (!string.IsNullOrEmpty(requiredCondition))
        {
            if (ConditionManager.Instance.CheckCondition(requiredCondition))
            {
                UnlockAndOpen();
                return;
            }
            else 
            { 
                HandleLockedDoor(false); 
                return; 
            }
        }

        HandleLockedDoor(false);
    }

    private bool HasProtectingPlanks()
    {
        if (protectingPlankIDs == null || protectingPlankIDs.Count == 0)
            return false;
        foreach (string id in protectingPlankIDs)
        {
            if (!SaveSystem.IsPlankPried(id)) 
                return true;
        }
        return false;
    }

    private bool ProtectingPlanksRemoved()
    {
        if (protectingPlankIDs == null || protectingPlankIDs.Count == 0)
            return false;
        foreach (string id in protectingPlankIDs)
        {
            if (!SaveSystem.IsPlankPried(id)) 
                return false;
        }
        return true;
    }

    public void UnlockAndOpen()
    {
        isLocked = false;

        if (!string.IsNullOrEmpty(doorID))
        {
            SaveSystem.SaveDoorUnlocked(doorID);
            SaveSystem.Save();
        }

        ToggleDoor();
    }

    private void ToggleDoor()
    {
        isDoorOpen = !isDoorOpen;

        if (isDoorOpen == true)
        {
            targetRotation = Quaternion.Euler(0f, openAngle, 0f);
            PlaySound(openSound);
        }
        else
        {
            targetRotation = Quaternion.Euler(0f, closeAngle, 0f);
            PlaySound(closeSound);
        }

        isAnimating = true;

        if (isDoorOpen == true && isLocked == false && string.IsNullOrEmpty(doorID) == false)
        {
            SaveSystem.SaveDoorUnlocked(doorID);
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
    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null) 
            audioSource.PlayOneShot(clip);
    }

    private void HandleLockedDoor(bool isKeyMissing)
    {
        PlaySound(lockedSound);

        string msg; 

        if (isKeyMissing == true)
        {
            msg = "Locked! I need a key.";
        }
        else
        {
            
            if (lockedMessageKey.IsEmpty == true)
            {
                msg = "Locked.";
            }
            else
            {
                msg = lockedMessageKey.GetLocalizedString();
            }
        }
        ShowTemporaryMessage(msg);
    }

    private void ShowTemporaryMessage(string text)
    {
        if (messagePanel != null && messageText != null)
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
        if (messagePanel != null) 
            messagePanel.SetActive(false);
    }

    public KeyCode GetInteractKey() => interactionKey;
}
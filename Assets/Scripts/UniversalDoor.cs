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
    public Key requiredKey;
    public string requiredCondition;

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

        // Megnövelt várakozás a mentés betöltéséhez
        yield return new WaitForSeconds(0.1f);

        string rewardState = DialogueManager.Instance != null ? DialogueManager.Instance.GetVariableState("got_reward") : "";
        bool wasUnlockedBySave = !string.IsNullOrEmpty(doorID) && SaveSystem.IsDoorUnlocked(doorID);

        // Ha az öreg már odaadta a kulcsot VAGY a mentés szerint nyitva van
        if (rewardState.ToLower() == "true" || wasUnlockedBySave)
        {
            isLocked = false;
            isDoorOpen = true;
            doorPivot.localRotation = Quaternion.Euler(0f, openAngle, 0f);
            targetRotation = doorPivot.localRotation;
            Debug.Log($"<color=green>AJTÓ HELYREÁLLÍTVA: {doorID}</color>");
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
        // 1. Ha már fel van oldva, csak nyitjuk-csukjuk
        if (!isLocked)
        {
            ToggleDoor();
            return;
        }

        // 2. HA ZÁRVA VAN (isLocked = true), nézzük meg, van-e kulcs/feltétel
        if (requiredKey != null)
        {
            if (KeyInventory.Instance.HasKey(requiredKey))
            {
                KeyInventory.Instance.RemoveKey(requiredKey);
                UnlockAndOpen();
                return;
            }
            else { HandleLockedDoor(true); return; }
        }

        if (!string.IsNullOrEmpty(requiredCondition))
        {
            if (ConditionManager.Instance.CheckCondition(requiredCondition))
            {
                UnlockAndOpen();
                return;
            }
            else { HandleLockedDoor(false); return; }
        }

       
        HandleLockedDoor(false);
    }

    private void UnlockAndOpen()
    {
        isLocked = false;

        if (!string.IsNullOrEmpty(doorID))
        {
            // 1. Betesszük a memóriába (a listába)
            SaveSystem.SaveDoorUnlocked(doorID);

            // 2. AZONNAL kiíratjuk a fájlba a lemezre!
            SaveSystem.Save();

            Debug.Log("<color=cyan>AJTÓ SIKERESEN RÖGZÍTVE A FÁJLBA: </color>" + doorID);
        }
        else
        {
            Debug.LogError("HIÁNYZIK A doorID! Adj nevet az ajtónak az Inspectorban!");
        }

        ToggleDoor();
    }

    private void ToggleDoor()
    {
        isDoorOpen = !isDoorOpen;
        targetRotation = Quaternion.Euler(0f, isDoorOpen ? openAngle : closeAngle, 0f);
        isAnimating = true;
        PlaySound(isDoorOpen ? openSound : closeSound);

        // BIZTONSÁGI MENTÉS: Ha kinyitottad, és nem volt zárolva, akkor is mentsük el az állapotát
        if (isDoorOpen && !isLocked && !string.IsNullOrEmpty(doorID))
        {
            SaveSystem.SaveDoorUnlocked(doorID);
            // Itt nem hívunk Save-et, hogy ne szaggasson, de a lista frissül
        }
    }


    public string GetInteractText() => isDoorOpen ? textCloseDoor.GetLocalizedString() : textOpenDoor.GetLocalizedString();

   

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null) audioSource.PlayOneShot(clip);
    }

    private void HandleLockedDoor(bool isKeyMissing)
    {
        PlaySound(lockedSound);
        string msg = isKeyMissing ? "Locked! I need a key." : (lockedMessageKey.IsEmpty ? "Locked." : lockedMessageKey.GetLocalizedString());
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
        if (messagePanel != null) messagePanel.SetActive(false);
    }

    public KeyCode GetInteractKey() => interactionKey;
}
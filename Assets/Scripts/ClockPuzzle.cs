using UnityEngine;
using TMPro;
using System.Collections;

public class ClockPuzzle : MonoBehaviour, InterfaceInteractable
{
    [Header("Save & Puzzle ID")]
    public string puzzleID = "ClockPuzzle";

    [Header("UI & Camera References")]
    public GameObject viewerPanel;
    public Camera mainPlayerCamera;
    public Camera puzzleCamera;

    [Header("UI Feedback")]
    public TextMeshProUGUI feedbackText;

    [Header("HUD Elements To Hide")]
    public GameObject[] hudElementsToHide;

    [Header("Player Control")]
    public OwnMovement playerController;
    public Interaction playerInteraction;

    [Header("Clock Parts")]
    public Transform hourHandPivot;
    public Transform minuteHandPivot;

    [Header("Puzzle Settings")]
    public float targetHourAngle = 90f;
    public float targetMinuteAngle = 90f;
    public float tolerance = 15f;

    [Header("Doors to unlock")]
    public UniversalDoor[] doorsToUnlock;
    public float delayBeforeUnlock = 1f;
    [Range(0f, 1f)] public float doorOpenVolume = 0.5f;

    [Header("Audio Events")]
    public AudioSource bedroomRadio;

    private bool isViewing = false;
    public bool isSolved = false;

    private void Start()
    {
        if (viewerPanel != null && viewerPanel != gameObject) viewerPanel.SetActive(false);
        if (puzzleCamera != null) puzzleCamera.gameObject.SetActive(false);

        StartCoroutine(CheckStatusAfterLoad());
    }

    private IEnumerator CheckStatusAfterLoad()
    {
        yield return new WaitForSeconds(0.1f);

        if (SaveSystem.IsPuzzleSolved(puzzleID))
        {
            isSolved = true;

            if (doorsToUnlock != null)
            {
                foreach (UniversalDoor door in doorsToUnlock)
                {
                    if (door != null)
                    {
                        door.UnlockOnly();
                    }
                }
            }

            bool clueAlreadySeen = ConditionManager.Instance != null && ConditionManager.Instance.CheckCondition("SawBedroomClue");

            if (bedroomRadio != null)
            {
                if (!clueAlreadySeen)
                {
                    if (!bedroomRadio.isPlaying)
                    {
                        bedroomRadio.time = 0f;
                        bedroomRadio.Play();
                    }
                }
                else
                {
                    bedroomRadio.Stop();
                }
            }
        }
    }

    private void Update()
    {
        if (isViewing)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                ClosePuzzle();
            }

            if (!isSolved)
            {
                CheckTime();
            }
        }
    }

    public string GetInteractText()
    {
        if (isSolved == true)
        {
            return "The clock is working.";
        }
        else
        {
            return "Examine the clock";
        }
    }
    public KeyCode GetInteractKey() => KeyCode.F;

    public void Interact()
    {
        if (isSolved || isViewing) return;
        OpenPuzzle();
    }

    public void OpenPuzzle()
    {
        if (isViewing || isSolved) return;

        isViewing = true;
        UiManager.isMenuOpen = true;

        if (mainPlayerCamera != null) mainPlayerCamera.gameObject.SetActive(false);
        if (puzzleCamera != null) puzzleCamera.gameObject.SetActive(true);
        if (viewerPanel != null) viewerPanel.SetActive(true);

        if (feedbackText != null)
        {
            feedbackText.text = "Set the correct time.";
            feedbackText.color = Color.white;
        }

        foreach (GameObject hudItem in hudElementsToHide)
        {
            if (hudItem != null) hudItem.SetActive(false);
        }

        if (playerInteraction != null) playerInteraction.enabled = false;
        if (playerController != null)
        {
            Rigidbody playerRb = playerController.GetComponent<Rigidbody>();
            if (playerRb != null) playerRb.linearVelocity = Vector3.zero;
            playerController.enabled = false;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ClosePuzzle()
    {
        if (!isViewing) return;

        isViewing = false;
        UiManager.isMenuOpen = false;

        if (mainPlayerCamera != null) mainPlayerCamera.gameObject.SetActive(true);
        if (puzzleCamera != null) puzzleCamera.gameObject.SetActive(false);
        if (viewerPanel != null) viewerPanel.SetActive(false);

        foreach (GameObject hudItem in hudElementsToHide)
        {
            if (hudItem != null) hudItem.SetActive(true);
        }

        if (playerInteraction != null) playerInteraction.enabled = true;
        if (playerController != null) playerController.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void CheckTime()
    {
        float currentHour = hourHandPivot.localEulerAngles.z;
        float currentMinute = minuteHandPivot.localEulerAngles.z;

        float hourDiff = Mathf.Abs(Mathf.DeltaAngle(currentHour, targetHourAngle));
        float minuteDiff = Mathf.Abs(Mathf.DeltaAngle(currentMinute, targetMinuteAngle));

        if (hourDiff <= tolerance && minuteDiff <= tolerance)
        {
            isSolved = true;
            SaveSystem.SavePuzzleSolved(puzzleID);
            StartCoroutine(PuzzleSolvedRoutine());
        }
    }

    private IEnumerator PuzzleSolvedRoutine()
    {
        if (feedbackText != null)
        {
            feedbackText.text = "Loud clicking noise";
            feedbackText.color = Color.red;
        }

        yield return new WaitForSeconds(delayBeforeUnlock);

        if (doorsToUnlock != null)
        {
            foreach (UniversalDoor door in doorsToUnlock)
            {
                if (door != null)
                {
                    AudioSource doorAudio = door.GetComponent<AudioSource>();
                    if (doorAudio != null)
                    {
                        doorAudio.volume = doorOpenVolume*0.5f;
                        doorAudio.spatialBlend = 1.0f;
                    }

                    door.UnlockAndPlaySound();
                }
            }
        }

        if (bedroomRadio != null)
        {
            bedroomRadio.time = 0f;
            bedroomRadio.Play();
        }

        yield return new WaitForSeconds(1.5f);

        ClosePuzzle();
    }
}
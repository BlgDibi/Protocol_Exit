using UnityEngine;
using TMPro;
using System.Collections;

public class FirstDoor : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] private Transform doorPivot;
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float closeAngle = 0f;
    [SerializeField] private float rotationSpeed = 2f;

    [Header("Message Settings")]
    [SerializeField] private GameObject subtitlePanel;
    [SerializeField] private TextMeshProUGUI subtitleText;
    [SerializeField] private string lockedMessage = "You have to find something"; 
    [SerializeField] private float messageDuration = 3f;

    [Header("Interaction UI")]
    [SerializeField] private GameObject promptCanvas;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip lockedSound;

    private bool isDoorOpen = false;
    private bool isUnlocked = false; 
    private bool isAnimating = false;
    private bool playerInRange = false;
    private Quaternion targetRotation;

    private void Start()
    {
        targetRotation = Quaternion.Euler(0f, closeAngle, 0f);
        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        if (promptCanvas != null) promptCanvas.SetActive(false);
        if (subtitlePanel != null) subtitlePanel.SetActive(false);
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
   
   
        if (playerInRange && Input.GetKeyDown(KeyCode.F))
        {
            if (!isDoorOpen)
            {
                if (isUnlocked)
                {
                    OpenDoor();
                }
                else
                {
                    if (lockedSound != null) audioSource.PlayOneShot(lockedSound);
                    StartCoroutine(ShowSubtitle());
                }
            }
        }
    }

    public void KeypadSuccess()
    {
        if (!isUnlocked)
        {
            isUnlocked = true;
        }
    }

    private void OpenDoor()
    {
        isDoorOpen = true;
        targetRotation = Quaternion.Euler(0f, openAngle, 0f);
        isAnimating = true;

        if (openSound != null) audioSource.PlayOneShot(openSound);

        if (promptCanvas != null) promptCanvas.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (promptCanvas != null && !isDoorOpen) promptCanvas.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (promptCanvas != null) promptCanvas.SetActive(false);
            if (subtitlePanel != null) subtitlePanel.SetActive(false);
        }
    }

    private IEnumerator ShowSubtitle()
    {
        if (subtitlePanel != null && subtitleText != null)
        {
            subtitleText.text = lockedMessage;
            subtitlePanel.SetActive(true);
            yield return new WaitForSeconds(messageDuration);
            subtitlePanel.SetActive(false);
        }
    }
}
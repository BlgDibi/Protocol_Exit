using UnityEngine;
using TMPro;

public class KeyPadSystem : MonoBehaviour
{
    [Header("Code Settings")]
    [Tooltip("Generated Code")]
    public bool useGeneratedCode = false;

    [Tooltip("Custom Code")]
    public string customCode = "1234";

    [Header("UI Settings")]
    public GameObject keypadCanvas;
    public TMP_Text display;

    [Header("Target Door")]
    public A1_Door doorToOpen_Old;
    public UniversalDoor doorToOpen_New;

    [Header("Feedback Colors")]
    public Color successColor = Color.green;
    public Color errorColor = Color.red;

    [Header("Player Reference")]
    [Tooltip("OwnMovement script")]
    public OwnMovement playerController;

    [Header("Animation Settings")]
    public Animator playerAnimator;
    public string typeAnimTrigger = "TypeCode";
    public float typeAnimLength = 2.0f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip buttonClickSound;
    public AudioClip grantedSound;
    public AudioClip deniedSound;

    private string currentInput = "";
    private Color originalColor;
    private bool isProcessing = false;

    private void Awake()
    {
        if (display != null)
        {
            originalColor = display.color;
        }
    }

    private void Start()
    {
        if (keypadCanvas != null)
        {
            keypadCanvas.SetActive(false);
        }

        if (playerAnimator == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerAnimator = player.GetComponentInChildren<Animator>();
        }
    }

    public void ShowKeypad()
    {
        UiManager.isMenuOpen = true;

        if (playerController != null) playerController.enabled = false;

        keypadCanvas.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        isProcessing = false;
        currentInput = "";
        UpdateDisplay();
    }

    public void CloseKeypad()
    {
        UiManager.isMenuOpen = false;
        keypadCanvas.SetActive(false);

        if (playerController != null) playerController.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ButtonPress(string digit)
    {
        if (isProcessing) return;

        if (currentInput.Length < 4)
        {
            PlaySound(buttonClickSound);
            currentInput += digit;
            UpdateDisplay();
        }
    }

    public void ClearInput()
    {
        if (isProcessing) return;

        currentInput = "";
        UpdateDisplay();
    }

    public void EnterCode()
    {
        if (isProcessing || currentInput.Length == 0) return;

        isProcessing = true;
        string targetCode = "";

        if (useGeneratedCode == true)
        {
            if (string.IsNullOrEmpty(SaveSystem.KeypadFullCode) == true)
            {
                targetCode = "0000";
            }
            else
            {
                targetCode = SaveSystem.KeypadFullCode;
            }
        }
        else
        {
            targetCode = customCode;
        }

        if (currentInput == targetCode)
        {
            PlaySound(grantedSound);
            display.text = "GRANTED";
            display.color = successColor;

            Invoke(nameof(HideUIForAnimation), 0.5f);

            Invoke(nameof(PlayTypingAnimation), 0.0f);

            Invoke(nameof(OpenLinkedDoors), 0.5f + typeAnimLength);

            Invoke(nameof(EnablePlayerMovement), 0.5f + typeAnimLength);
        }
        else
        {
            PlaySound(deniedSound);
            display.text = "DENIED";
            display.color = errorColor;

            Invoke(nameof(ResetAfterError), 1f);
        }
    }

    private void HideUIForAnimation()
    {
        UiManager.isMenuOpen = false;
        keypadCanvas.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void EnablePlayerMovement()
    {
        if (playerController != null)
        {
            playerController.enabled = true;
        }
    }

    private void PlayTypingAnimation()
    {
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger(typeAnimTrigger);
        }
    }

    private void OpenLinkedDoors()
    {
        if (doorToOpen_Old != null)
        {
            doorToOpen_Old.Unlock();
        }

        if (doorToOpen_New != null)
        {
            doorToOpen_New.UnlockAndOpen();
        }
    }

    private void ResetAfterError()
    {
        currentInput = "";
        UpdateDisplay();
        isProcessing = false;
    }

    private void UpdateDisplay()
    {
        if (display != null)
        {
            display.text = currentInput;
            display.color = originalColor;
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
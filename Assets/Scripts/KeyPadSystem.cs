using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class KeyPadSystem : MonoBehaviour
{
    [Header("Settings")]
    public string correctCode = "5821";
    public GameObject keypadCanvas;
    public TMP_Text display;
    public UniversalDoor doorToOpen;

    [Header("Feedback Colors")]
    public Color successColor = Color.green;
    public Color errorColor = Color.red;

    [Header("Player Control")]
    public MonoBehaviour playerController;

    private string currentInput = "";
    private Color originalColor;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip buttonClickSound;
    public AudioClip grantedSound;
    public AudioClip deniedSound;

    void Awake()
    {
        if (display != null)
        {
            originalColor = display.color;
        }
    }

    void Start()
    {
        if (keypadCanvas != null) keypadCanvas.SetActive(false);
    }

    public void ShowKeypad()
    {
        if (playerController != null) playerController.enabled = false;

        keypadCanvas.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        currentInput = "";
        UpdateDisplay();
    }

    public void CloseKeypad()
    {
        keypadCanvas.SetActive(false);

        if (playerController != null) playerController.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ButtonPress(string digit)
    {
        if (currentInput.Length < 4)
        {
            PlayButtonSound();

            currentInput += digit;
            UpdateDisplay();
        }
    }

    public void ClearInput()
    {
        currentInput = "";
        UpdateDisplay();
    }

    public void EnterCode()
    {
        if (currentInput == correctCode)
        {
            if (audioSource != null && grantedSound != null)
                audioSource.PlayOneShot(grantedSound);

            display.text = "GRANTED";
            display.color = successColor;
            if (doorToOpen != null) doorToOpen.KeypadSuccess();
            Invoke("CloseKeypad", 1f);
        }
        else
        {
            if (audioSource != null && deniedSound != null)
                audioSource.PlayOneShot(deniedSound);

            display.text = "DENIED";
            display.color = errorColor;
            currentInput = "";
            Invoke("UpdateDisplay", 1f);
        }
    }

    void UpdateDisplay()
    {
        if (display != null)
        {
            display.text = currentInput;
            display.color = originalColor;
        }
    }

    private void PlayButtonSound()
    {
        if (audioSource != null && buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
    }

}
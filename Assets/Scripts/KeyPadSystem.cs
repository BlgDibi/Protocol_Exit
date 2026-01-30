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

    void Awake()
    {
        if (display != null)
        {
            originalColor = display.color;
        }
    }

    void Start()
    {
        keypadCanvas.SetActive(false);
    }

    public void ShowKeypad()
    {
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        keypadCanvas.SetActive(true);
        Canvas.ForceUpdateCanvases();

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
            display.text = "GRANTED";
            display.color = successColor;

            if (doorToOpen != null) doorToOpen.KeypadSuccess();
            Invoke("CloseKeypad", 1f);
        }
        else
        {
            display.text = "DENIED";
            display.color = errorColor;

            currentInput = "";
            Invoke("UpdateDisplay", 1f);
        }
    }

    void UpdateDisplay()
    {
        display.text = currentInput;

        display.color = originalColor;
    }
}
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;

public class KeyPadTrigger : MonoBehaviour, InterfaceInteractable
{
    public static bool IsPlayerInZone = false;

    [Header("Settings")]
    public KeyPadSystem uiKeypad;
    [SerializeField] private Image crosshair;

    [Header("Localization")]
    public LocalizedString interactionPrompt;

    [SerializeField] private KeyCode interactkey = KeyCode.F;

    private void Start()
    {
        if (uiKeypad != null)
        {
            uiKeypad.InitializeUVNumbers();
        }
    }

    public string GetInteractText()
    {
        return interactionPrompt.GetLocalizedString();
    }

    public KeyCode GetInteractKey()
    {
        return interactkey;
    }

    public void Interact()
    {
        if (uiKeypad != null && !uiKeypad.keypadCanvas.activeSelf)
        {
            HighlightCrosshair(false);
            uiKeypad.ShowKeypad();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            IsPlayerInZone = true;
            HighlightCrosshair(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            IsPlayerInZone = false;
            HighlightCrosshair(false);
            if (uiKeypad != null) uiKeypad.CloseKeypad();
        }
    }

    void HighlightCrosshair(bool on)
    {
        if (crosshair != null)
        {
            if (on == true)
            {
                crosshair.color = Color.red;
            }
            else
            {
                crosshair.color = Color.white;
            }
        }
    }
}
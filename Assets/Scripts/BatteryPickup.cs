using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;

public class BatteryPickup : MonoBehaviour, InterfaceInteractable
{
    [Header("Battery Settings")]
    public float batteryRestoreAmount = 10f;

    [Header("Settings")]
    [SerializeField] private Image crosshair;

    [Header("Localization")]
    public LocalizedString interactionPrompt;
    [SerializeField] private KeyCode interactkey = KeyCode.E;

    public string GetInteractText() => interactionPrompt.GetLocalizedString();
    public KeyCode GetInteractKey() => interactkey;

    public void Interact()
    {
        FlashlightController equipSystem = FindFirstObjectByType<FlashlightController>();

        if (equipSystem != null)
        {
            equipSystem.AddBattery(batteryRestoreAmount);
        }

        HighlightCrosshair(false);

       
        gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other) { if (other.CompareTag("Player")) HighlightCrosshair(true); }
    void OnTriggerExit(Collider other) { if (other.CompareTag("Player")) HighlightCrosshair(false); }

    void HighlightCrosshair(bool on)
    {
        if (crosshair != null) crosshair.color = on ? Color.red : Color.white;
    }
}
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;

public class Flashlightpickup : MonoBehaviour, InterfaceInteractable
{
    public static bool HasFlashlight = false;

    [Header("Flashlight Settings")]
    public GameObject flashlightInHand;
    public GameObject flashlightOnTable;

    [Header("Settings")]
    [SerializeField] private Image crosshair;

    [Header("Localization")]
    public LocalizedString interactionPrompt;
    [SerializeField] private KeyCode interactkey = KeyCode.E;

    void Start()
    {
        if (HasFlashlight && flashlightInHand != null)
        {
            flashlightInHand.SetActive(true);
            if (flashlightOnTable != null) flashlightOnTable.SetActive(false);
        }
    }

    public string GetInteractText() => interactionPrompt.GetLocalizedString();
    public KeyCode GetInteractKey() => interactkey;

    public void Interact()
    {
        HasFlashlight = true;
        ApplyFlashlightState();
    }

    private void ApplyFlashlightState()
    {
        if (flashlightInHand != null) flashlightInHand.SetActive(true);
        if (flashlightOnTable != null) flashlightOnTable.SetActive(false);
        else gameObject.SetActive(false);

        HighlightCrosshair(false);
    }

    void OnTriggerEnter(Collider other) { if (other.CompareTag("Player")) HighlightCrosshair(true); }
    void OnTriggerExit(Collider other) { if (other.CompareTag("Player")) HighlightCrosshair(false); }

    void HighlightCrosshair(bool on)
    {
        if (crosshair != null) crosshair.color = on ? Color.red : Color.white;
    }
}
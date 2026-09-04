using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using System.Collections;

public class Flashlightpickup : MonoBehaviour, InterfaceInteractable
{
    public static bool HasFlashlight = false;

    [Header("Flashlight Settings")]
    public GameObject flashlightOnTable;

    [Header("Tutorial UI")]
    public GameObject tutorialTextPanel;

    [Header("Settings")]
    [SerializeField] private Image crosshair;

    [Header("Localization")]
    public LocalizedString interactionPrompt;
    [SerializeField] private KeyCode interactkey = KeyCode.E;

    void Start()
    {
        if (HasFlashlight && flashlightOnTable != null)
        {
            flashlightOnTable.SetActive(false);
        }
    }

    public string GetInteractText() => interactionPrompt.GetLocalizedString();
    public KeyCode GetInteractKey() => interactkey;

    public void Interact()
    {
        HasFlashlight = true;

        if (flashlightOnTable != null) flashlightOnTable.SetActive(false);
        else gameObject.SetActive(false);

        HighlightCrosshair(false);

        if (UiManager.Instance != null)
        {
            UiManager.Instance.AddFlashlightToUI();

            UiManager.Instance.ShowTemporaryMessage(tutorialTextPanel, 5f);
        }

        FlashlightController equipSystem = FindFirstObjectByType<FlashlightController>();
        if (equipSystem != null)
        {
            equipSystem.EquipItemAutomatically(1);
        }
    }

    void OnTriggerEnter(Collider other) { if (other.CompareTag("Player")) HighlightCrosshair(true); }
    void OnTriggerExit(Collider other) { if (other.CompareTag("Player")) HighlightCrosshair(false); }

    void HighlightCrosshair(bool on)
    {
        if (crosshair != null) crosshair.color = on ? Color.red : Color.white;
    }
}
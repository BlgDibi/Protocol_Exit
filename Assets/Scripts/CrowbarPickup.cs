using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;

public class CrowbarPickup : MonoBehaviour, InterfaceInteractable
{
    public static bool HasCrowbar = false;

    [Header("Crowbar Settings")]
    public GameObject crowbarOnTable; 

    [Header("Settings")]
    [SerializeField] private Image crosshair;

    [Header("Localization")]
    public LocalizedString interactionPrompt;
    [SerializeField] private KeyCode interactkey = KeyCode.E;

    void Start()
    {
        if (HasCrowbar && crowbarOnTable != null)
        {
            crowbarOnTable.SetActive(false);
        }
    }

    public string GetInteractText() => interactionPrompt.GetLocalizedString();
    public KeyCode GetInteractKey() => interactkey;

    public void Interact()
    {
        HasCrowbar = true;

        if (crowbarOnTable != null) crowbarOnTable.SetActive(false);
        else gameObject.SetActive(false);

        HighlightCrosshair(false);

        if (UiManager.Instance != null)
        {
            UiManager.Instance.AddCrowbarToUI();
        }

      
        FlashlightController equipSystem = FindFirstObjectByType<FlashlightController>();
        if (equipSystem != null)
        {
            equipSystem.UpdateEquipmentState();
        }
    }

    void OnTriggerEnter(Collider other) { if (other.CompareTag("Player")) HighlightCrosshair(true); }
    void OnTriggerExit(Collider other) { if (other.CompareTag("Player")) HighlightCrosshair(false); }

    void HighlightCrosshair(bool on)
    {
        if (crosshair != null) crosshair.color = on ? Color.red : Color.white;
    }
}
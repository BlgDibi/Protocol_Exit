using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using StarterAssets;

public class Flashlightpickup : MonoBehaviour, InterfaceInteractable
{
    public static bool IsPlayerInZone = false;

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
        if (HasFlashlight)
        {
            ApplyFlashlightState();
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
        HighlightCrosshair(false);
        HasFlashlight = true; 
        ApplyFlashlightState();
    }

    private void ApplyFlashlightState()
    {
        if (flashlightInHand != null)
        {
            flashlightInHand.SetActive(true);

            ThirdPersonController controller = flashlightInHand.GetComponentInParent<ThirdPersonController>();
            if (controller != null)
            {
                controller.isFlashlightMode = true;
                controller.flashlightObject = flashlightInHand;
                Debug.Log("Flashlight mode activated");
            }
        }

        if (flashlightOnTable != null)
        {
            flashlightOnTable.SetActive(false);
        }
        else
        {
            gameObject.SetActive(false);
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
        }
    }

    void HighlightCrosshair(bool on)
    {
        if (crosshair != null) crosshair.color = on ? Color.red : Color.white;
    }
}
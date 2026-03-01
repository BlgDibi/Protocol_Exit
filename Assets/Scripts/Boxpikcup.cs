using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;

public class Boxpickup : MonoBehaviour, InterfaceInteractable
{
    [Header("Box info")]
    [SerializeField] private SulyDobozData dobozData;

    [Header("Settings")]
    [SerializeField] private KeyCode interactkey = KeyCode.E;
    [SerializeField] private Image crosshair;

    [Header("Localization")]
    public LocalizedString interactionPrompt;
    [SerializeField] private string fallbackText = "Box Pickup";

    public string GetInteractText()
    {
       
            return interactionPrompt.GetLocalizedString();
        
     
    }

    public KeyCode GetInteractKey() => interactkey;

    public void Interact()
    {
        if (dobozData != null)
        {
            BoxInventory.Instance.AddBox(dobozData);

            HighlightCrosshair(false);

            gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) HighlightCrosshair(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) HighlightCrosshair(false);
    }

    void HighlightCrosshair(bool on)
    {
        if (crosshair != null) crosshair.color = on ? Color.red : Color.white;
    }
}
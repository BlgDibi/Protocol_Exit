using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;

public class DiningItemsPickup : MonoBehaviour, InterfaceInteractable
{
    [Header("Item ID-s")]
    public string itemID;


    public static System.Collections.Generic.List<string> CollectedDiningItems = new System.Collections.Generic.List<string>();

    [Header("UI Crosshair")]
    [SerializeField] private Image crosshair;

    [Header("Localization")]
    public LocalizedString interactionPrompt;
    [SerializeField] private KeyCode interactkey = KeyCode.E;

    private void Start()
    {
        if (CollectedDiningItems.Contains(itemID))
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }

    public string GetInteractText() => interactionPrompt.GetLocalizedString();
    public KeyCode GetInteractKey() => interactkey;

    public void Interact()
    {
        if (!CollectedDiningItems.Contains(itemID))
        {
            CollectedDiningItems.Add(itemID);
        }

        if (UiManager.Instance != null)
        {
            UiManager.Instance.AddDiningItemToUI(itemID);
        }

        HighlightCrosshair(false);
        gameObject.SetActive(false);
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
        if (crosshair != null)
        {
            crosshair.color = on ? Color.red : Color.white;
        }
    }
}
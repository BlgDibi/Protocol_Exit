using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;

public class ValvePickup : MonoBehaviour, InterfaceInteractable
{
    public static bool HasValve = false;

    [Header("Valve Settings")]
    [Tooltip("Valve Model")]
    public GameObject valveModel;

    [Header("Settings")]
    [SerializeField] private Image crosshair;

    [Header("Localization")]
    public LocalizedString interactionPrompt;
    [SerializeField] private KeyCode interactkey = KeyCode.E;

    void Start()
    {
        if (HasValve && valveModel != null)
        {
            valveModel.SetActive(false);
        }
    }

    public string GetInteractText() => interactionPrompt.GetLocalizedString();
    public KeyCode GetInteractKey() => interactkey;

    public void Interact()
    {
        HasValve = true;

        if (valveModel != null) valveModel.SetActive(false);
        else gameObject.SetActive(false);

        HighlightCrosshair(false);

        if (UiManager.Instance != null)
        {
            UiManager.Instance.AddValveToUI();
        }

        if (ConditionManager.Instance != null)
        {
            ConditionManager.Instance.SetCondition("HasValve", true);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            HighlightCrosshair(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            HighlightCrosshair(false);
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
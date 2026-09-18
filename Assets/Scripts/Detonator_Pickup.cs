using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;

public class Detonator_Pickup : MonoBehaviour, InterfaceInteractable
{
    public static bool HasDetonator = false;

    [Header("Detonator Settings")]
    public GameObject detonatorOnFloor;

    [Header("Settings")]
    [SerializeField] private Image crosshair;

    [Header("Localization")]
    public LocalizedString interactionPrompt;
    [SerializeField] private KeyCode interactkey = KeyCode.E;

    void Start()
    {
        if (HasDetonator == true && detonatorOnFloor != null)
        {
            detonatorOnFloor.SetActive(false);
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
        HasDetonator = true;

        if (detonatorOnFloor != null)
        {
            detonatorOnFloor.SetActive(false);
        }
        else
        {
            gameObject.SetActive(false);
        }

        HighlightCrosshair(false);

        if (UiManager.Instance != null)
        {
            UiManager.Instance.AddDetonatorToUI();
        }

        if (ConditionManager.Instance != null)
        {
            ConditionManager.Instance.SetCondition("HasDetonator", true);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") == true)
            HighlightCrosshair(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") == true)
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
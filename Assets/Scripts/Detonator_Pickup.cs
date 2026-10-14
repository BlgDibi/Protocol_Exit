using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;

public class Detonator_Pickup : MonoBehaviour, InterfaceInteractable
{
    public static bool HasDetonator = false;

    [Header("Detonator Settings")]
    public GameObject detonatorOnFloor;

    [Tooltip("Detonator mesh render")]
    public MeshRenderer detonatorRenderer;

    [Tooltip("Flashlight controller")]
    public FlashlightController flashlightController;

    [Header("Settings")]
    [SerializeField] private Image crosshair;

    [Header("Localization")]
    public LocalizedString interactionPrompt;
    [SerializeField] private KeyCode interactkey = KeyCode.E;

    private Color uvColor;
    private Color normalColor = Color.black;

    void Start()
    {
        if (HasDetonator == true && detonatorOnFloor != null)
        {
            detonatorOnFloor.SetActive(false);
        }

        if (flashlightController == null)
        {
            flashlightController = FindObjectOfType<FlashlightController>();
        }

        if (detonatorRenderer != null)
        {
            ColorUtility.TryParseHtmlString("#00FF33", out uvColor);
            uvColor = uvColor * 2.5f;

            detonatorRenderer.material.EnableKeyword("_EMISSION");
            detonatorRenderer.material.SetColor("_EmissionColor", normalColor);
        }
    }

    private bool IsUVLightActive()
    {
        if (flashlightController == null) return false;

        bool isHoldingFlashlight = flashlightController.flashlightInHand != null && flashlightController.flashlightInHand.activeSelf;
        bool isLightOn = flashlightController.flashlightLightComponent != null && flashlightController.flashlightLightComponent.enabled;

        return isHoldingFlashlight && isLightOn && flashlightController.isUVMode;
    }

    void Update()
    {
        if (detonatorRenderer == null) return;

        if (IsUVLightActive())
        {
            detonatorRenderer.material.SetColor("_EmissionColor", uvColor);
        }
        else
        {
            detonatorRenderer.material.SetColor("_EmissionColor", normalColor);
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
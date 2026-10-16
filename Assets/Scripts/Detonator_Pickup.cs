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

    [Header("Settings")]
    [SerializeField] private Image crosshair;

    [Header("Localization")]
    public LocalizedString interactionPrompt;
    [SerializeField] private KeyCode interactkey = KeyCode.E;

    private Color highlightColor;
    private Color normalColor = Color.black;

    private bool isHovered = false;

    void Start()
    {
        bool isBarricadeDestroyed = false;
        if (ConditionManager.Instance != null)
        {
            isBarricadeDestroyed = ConditionManager.Instance.CheckCondition("BarricadeDestroyed");
        }

        if ((HasDetonator == true || isBarricadeDestroyed == true) && detonatorOnFloor != null)
        {
            detonatorOnFloor.SetActive(false);
            gameObject.SetActive(false);
        }

        if (detonatorRenderer != null)
        {
            ColorUtility.TryParseHtmlString("#32A645", out highlightColor);
            highlightColor = highlightColor * 1.5f;

            detonatorRenderer.material.EnableKeyword("_EMISSION");
            detonatorRenderer.material.SetColor("_EmissionColor", normalColor);
        }
    }

    void Update()
    {
        if (detonatorRenderer == null) return;

        if (isHovered)
        {
            detonatorRenderer.material.SetColor("_EmissionColor", highlightColor);
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
        {
            isHovered = true;
            HighlightCrosshair(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") == true)
        {
            isHovered = false;
            HighlightCrosshair(false);
        }
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
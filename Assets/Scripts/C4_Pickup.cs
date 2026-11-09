using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;

public class C4_Pickup : MonoBehaviour, InterfaceInteractable
{
    public static bool HasC4 = false;

    [Header("C4 Settings")]
    public GameObject c4InHand;
    public Animator mechanicAnimator;
    public string ReturnToIdleTrigger = "BackToIdle";

    [Header("Collider Settings")]
    [Tooltip("Collider size after detonator")]
    public Vector3 expandedColliderSize = new Vector3(3f, 3f, 3f); 
    private BoxCollider itemCollider;

    [Header("Settings")]
    [SerializeField] private Image crosshair;

    [Header("Localization")]
    public LocalizedString interactionPrompt;
    [SerializeField] private KeyCode interactkey = KeyCode.E;

    void Awake()
    {
        itemCollider = GetComponent<BoxCollider>();
    }

    void Start()
    {
        if (HasC4 == true)
        {
            gameObject.SetActive(false);
        }
    }

    void OnEnable()
    {
        if (itemCollider != null && ConditionManager.Instance != null)
        {
            if (ConditionManager.Instance.CheckCondition("C4Spawned"))
            {
                itemCollider.size = expandedColliderSize;
            }
        }
    }

    public string GetInteractText()
    {
        if (ConditionManager.Instance != null && !ConditionManager.Instance.CheckCondition("C4Spawned"))
        {
            return "";
        }
        return interactionPrompt.GetLocalizedString();
    }

    public KeyCode GetInteractKey()
    {
        return interactkey;
    }

    public void Interact()
    {
        if (ConditionManager.Instance != null && !ConditionManager.Instance.CheckCondition("C4Spawned"))
        {
            return;
        }

        HasC4 = true;

        if (mechanicAnimator != null && !string.IsNullOrEmpty(ReturnToIdleTrigger))
        {
            mechanicAnimator.SetTrigger(ReturnToIdleTrigger);
        }

        gameObject.SetActive(false);
        HighlightCrosshair(false);

        if (ConditionManager.Instance != null)
        {
            ConditionManager.Instance.SetCondition("HasC4", true);
        }

        if (UiManager.Instance != null)
        {
            UiManager.Instance.AddC4ToUI();
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
            bool isSpawned = ConditionManager.Instance != null && ConditionManager.Instance.CheckCondition("C4Spawned");

            if (on == true && isSpawned)
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
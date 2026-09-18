using UnityEngine;
using UnityEngine.Localization;

public class Barricade : MonoBehaviour, InterfaceInteractable
{
    [Header("Localization")]
    public LocalizedString interactionPrompt;
    [SerializeField] private KeyCode interactkey = KeyCode.E;

    public Transform c4PlantPoint;

    void Awake()
    {
        if (ConditionManager.Instance != null && ConditionManager.Instance.CheckCondition("BarricadeDestroyed"))
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (ConditionManager.Instance != null && ConditionManager.Instance.CheckCondition("BarricadeDestroyed"))
        {
            Destroy(gameObject);
        }
    }

    public string GetInteractText()
    {
        if (C4_Pickup.HasC4)
            return interactionPrompt.GetLocalizedString();
        return "";
    }

    public KeyCode GetInteractKey() => interactkey;

    public void Interact()
    {
        if (C4_Pickup.HasC4)
        {
            C4_Plant plantManager = FindObjectOfType<C4_Plant>();
            if (plantManager != null)
            {
                plantManager.PlantC4AtSpot(c4PlantPoint);

                this.enabled = false;

                Collider col = GetComponent<Collider>();
                if (col != null) col.enabled = false;
            }
        }
    }
}
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
        if (C4_Pickup.HasC4 == true)
        {
            return interactionPrompt.GetLocalizedString();
        }
        else
        {
            return "";
        }
    }

    public KeyCode GetInteractKey()
    {
        return interactkey;
    }

    public void Interact()
    {
        if (C4_Pickup.HasC4 == true)
        {
            C4_Plant plantManager = FindFirstObjectByType<C4_Plant>();

            if (plantManager != null)
            {
                plantManager.PlantC4AtSpot(c4PlantPoint);

                this.enabled = false;

                Collider col = GetComponent<Collider>();
                if (col != null)
                {
                    col.enabled = false;
                }
            }
        }
    }
}
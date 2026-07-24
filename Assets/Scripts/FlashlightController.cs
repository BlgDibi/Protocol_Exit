using UnityEngine;

public class FlashlightController : MonoBehaviour
{
    [Header("Item References (in hand)")]
    public GameObject flashlightInHand;
    public GameObject crowbarInHand;

    [Header("Key Bindings ")]
    [Tooltip("With this button you switch between items")]
    [SerializeField] private KeyCode cycleKey = KeyCode.Q;

    [Tooltip("With this button you activate or deactivate the item")]
    [SerializeField] private KeyCode toggleKey = KeyCode.U;

    private int selectedItem = 1;

    private bool isHoldingItem = true;

    void Start()
    {
        UpdateEquipmentState();
    }

    void Update()
    {
        if (Input.GetKeyDown(cycleKey))
        {
            CycleItems();
        }

        if (Input.GetKeyDown(toggleKey))
        {
            isHoldingItem = !isHoldingItem;
            UpdateEquipmentState();
        }
    }

    private void CycleItems()
    {
        bool hasFlashlight = Flashlightpickup.HasFlashlight;
        bool hasCrowbar = CrowbarPickup.HasCrowbar;

        if (!hasFlashlight && !hasCrowbar) return;

        isHoldingItem = true;

        if (selectedItem == 1)
        {
            if (hasCrowbar) selectedItem = 2; 
        }
        else if (selectedItem == 2)
        {
            if (hasFlashlight) selectedItem = 1;
        }

        if (selectedItem == 1 && !hasFlashlight && hasCrowbar) selectedItem = 2;
        if (selectedItem == 2 && !hasCrowbar && hasFlashlight) selectedItem = 1;

        UpdateEquipmentState();
    }

    public void UpdateEquipmentState()
    {
        if (flashlightInHand != null) flashlightInHand.SetActive(false);
        if (crowbarInHand != null) crowbarInHand.SetActive(false);

        if (!isHoldingItem) return;

        if (selectedItem == 1 && Flashlightpickup.HasFlashlight && flashlightInHand != null)
        {
            flashlightInHand.SetActive(true);
        }
        else if (selectedItem == 2 && CrowbarPickup.HasCrowbar && crowbarInHand != null)
        {
            crowbarInHand.SetActive(true);
        }
    }
}
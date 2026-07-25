using UnityEngine;

public class FlashlightController : MonoBehaviour
{
    [Header("Item References (in hand)")]
    public GameObject flashlightInHand;
    public GameObject crowbarInHand;

    [Header("Key Bindings")]
    [Tooltip("With this button you switch between items")]
    [SerializeField] private KeyCode cycleKey = KeyCode.Q;

    [Tooltip("With this button you activate or deactivate the item")]
    [SerializeField] private KeyCode toggleKey = KeyCode.U;

    [Header("Battery Settings")]
    public float maxBattery = 100f;
    public static float currentBattery = 100f;
    [Tooltip("How much drain while active")]
    public float activeDrainRate = 2.0f;
    [Tooltip("How much drain while not active")]
    public float passiveDrainRate = 0.1f;

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
            if (!isHoldingItem && selectedItem == 1 && currentBattery <= 0 && Flashlightpickup.HasFlashlight)
            {
                Debug.Log("Flashlight has no power!");
                return;
            }

            isHoldingItem = !isHoldingItem;
            UpdateEquipmentState();
        }

        HandleBattery();
    }

    private void HandleBattery()
    {
        if (!Flashlightpickup.HasFlashlight) return;

        if (selectedItem == 1 && isHoldingItem && flashlightInHand != null && flashlightInHand.activeSelf)
        {
            currentBattery -= activeDrainRate * Time.deltaTime;

            if (currentBattery <= 0)
            {
                currentBattery = 0;
                isHoldingItem = false;
                UpdateEquipmentState();
            }
        }
        else
        {
            currentBattery -= passiveDrainRate * Time.deltaTime;
            if (currentBattery < 0) currentBattery = 0;
        }

        if (BatteryUI_Flashlight.Instance != null)
        {
            BatteryUI_Flashlight.Instance.UpdateBatteryUI(currentBattery, maxBattery);
        }
    }

    public void AddBattery(float amount)
    {
        currentBattery += amount;
        if (currentBattery > maxBattery) currentBattery = maxBattery;
    }

    public void EquipItemAutomatically(int itemIndex)
    {
        selectedItem = itemIndex;
        isHoldingItem = true;

        if (selectedItem == 1 && currentBattery <= 0)
        {
            isHoldingItem = false;
        }

        UpdateEquipmentState();
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

        if (selectedItem == 1 && currentBattery <= 0)
        {
            isHoldingItem = false;
        }

        UpdateEquipmentState();
    }

    public void UpdateEquipmentState()
    {
        if (flashlightInHand != null) flashlightInHand.SetActive(false);
        if (crowbarInHand != null) crowbarInHand.SetActive(false);

        if (BatteryUI_Flashlight.Instance != null) BatteryUI_Flashlight.Instance.SetVisible(false);

        if (!isHoldingItem) return;

        if (selectedItem == 1 && Flashlightpickup.HasFlashlight && flashlightInHand != null)
        {
            flashlightInHand.SetActive(true);

            if (BatteryUI_Flashlight.Instance != null) BatteryUI_Flashlight.Instance.SetVisible(true);
        }
        else if (selectedItem == 2 && CrowbarPickup.HasCrowbar && crowbarInHand != null)
        {
            crowbarInHand.SetActive(true);
        }
    }
}
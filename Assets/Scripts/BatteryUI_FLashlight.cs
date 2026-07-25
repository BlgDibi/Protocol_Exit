using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BatteryUI_Flashlight : MonoBehaviour
{
    public static BatteryUI_Flashlight Instance { get; private set; }

    [Header("UI Elements, Container")]
    [SerializeField] private GameObject uiContainer;
    [SerializeField] private TextMeshProUGUI batteryText;
    [Header("Battery Fill Bar")]
    [SerializeField] private Image batteryFillBar;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void UpdateBatteryUI(float currentBattery, float maxBattery)
    {
        if (batteryText != null)
        {
            batteryText.text = Mathf.RoundToInt(currentBattery).ToString() + "%";
        }

        if (batteryFillBar != null)
        {
            batteryFillBar.fillAmount = currentBattery / maxBattery;
        }
    }

    public void SetVisible(bool isVisible)
    {
        if (uiContainer != null)
        {
            uiContainer.SetActive(isVisible);
        }
    }

}
using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class FlashlightController : MonoBehaviour
{
    [Header("Item References (in hand)")]
    public GameObject flashlightInHand;
    public GameObject crowbarInHand;

    [Header("Key Bindings")]
    [SerializeField] private KeyCode cycleKey = KeyCode.Q;

    [Header("Animation Settings")]
    public Animator characterAnimator;
    public string drawFlashlightTrigger = "DrawItem";
    public string drawCrowbarTrigger = "DrawItemLeft";
    public string hideFlashlightTrigger = "HideItem";      
    public string hideCrowbarTrigger = "HideItemLeft";
    public float drawDelay = 0.5f;
    public float hideDelay = 0.3f;
    private bool isChangingItem = false;

    [Header("Battery Settings")]
    public float maxBattery = 100f;
    public static float currentBattery = 100f;
    public float activeDrainRate = 2.0f;
    public float passiveDrainRate = 0.1f;

    [Header("UV Light Settings")]
    public Color normalLightColor = Color.white;
    public Color uvLightColor = new Color(0.5f, 0f, 1f, 1f);
    [HideInInspector] public bool isUVMode = false;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip clickSound;

    [Header("Light Effects")]
    public Light flashlightLightComponent;
    private float originalIntensity;

    private int selectedItem = 1;
    private bool isHoldingItem = true;
    private bool isLightOn = true;

    void Start()
    {
        if (flashlightLightComponent != null)
        {
            originalIntensity = flashlightLightComponent.intensity;
            flashlightLightComponent.color = normalLightColor;
        }

        isHoldingItem = false;
        ApplyEquipmentVisibility();
    }

    void Update()
    {
        if (isChangingItem) return;

        if (Input.GetKeyDown(cycleKey) && !UiManager.isMenuOpen)
        {
            CycleItems();
        }

        if (Input.GetMouseButtonDown(1) && !UiManager.isMenuOpen)
        {
            if (!isHoldingItem && selectedItem == 1 && currentBattery <= 0 && Flashlightpickup.HasFlashlight)
            {
                Debug.Log("Flashlight has no power!");
                return;
            }

            if ((selectedItem == 1 && Flashlightpickup.HasFlashlight) || (selectedItem == 2 && CrowbarPickup.HasCrowbar))
            {
                isHoldingItem = !isHoldingItem;

                if (audioSource != null && clickSound != null)
                {
                    audioSource.PlayOneShot(clickSound);
                }

                UpdateEquipmentState();
            }
        }

        if (Input.GetMouseButtonDown(0) && !UiManager.isMenuOpen)
        {
            if (isHoldingItem && selectedItem == 1 && Flashlightpickup.HasFlashlight)
            {
                if (currentBattery > 0)
                {
                    isLightOn = !isLightOn;

                    if (flashlightLightComponent != null)
                        flashlightLightComponent.enabled = isLightOn;

                    if (audioSource != null && clickSound != null)
                        audioSource.PlayOneShot(clickSound);
                }
            }
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f && !UiManager.isMenuOpen && isHoldingItem && selectedItem == 1 && Flashlightpickup.HasFlashlight && isLightOn)
        {
            ToggleLightColor();
        }

        HandleBattery();
    }

    private void ToggleLightColor()
    {
        isUVMode = !isUVMode;
        if (flashlightLightComponent != null)
        {
            flashlightLightComponent.color = isUVMode ? uvLightColor : normalLightColor;
        }
        if (audioSource != null && clickSound != null) audioSource.PlayOneShot(clickSound);
    }

    private void HandleBattery()
    {
        if (!Flashlightpickup.HasFlashlight) return;

        if (selectedItem == 1 && isHoldingItem && isLightOn && flashlightInHand != null && flashlightInHand.activeSelf)
        {
            currentBattery -= activeDrainRate * Time.deltaTime;
            HandleLightEffects();

            if (currentBattery <= 0)
            {
                currentBattery = 0;
                isLightOn = false;
                if (flashlightLightComponent != null) flashlightLightComponent.enabled = false;
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

    private void HandleLightEffects()
    {
        if (flashlightLightComponent == null) return;
        float batteryPercent = currentBattery / maxBattery;

        if (batteryPercent <= 0.20f)
        {
            float newIntensity = originalIntensity * (batteryPercent / 0.20f);
            if (batteryPercent <= 0.15f)
            {
                float flicker = Random.Range(0.2f, 0.6f);
                newIntensity *= flicker;
            }
            flashlightLightComponent.intensity = newIntensity;
        }
        else
        {
            flashlightLightComponent.intensity = originalIntensity;
        }
    }

    public void AddBattery(float amount)
    {
        currentBattery += amount;
        if (currentBattery > maxBattery)
            currentBattery = maxBattery;
    }

    public void EquipItemAutomatically(int itemIndex)
    {
        if (!this.enabled) return;

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

        if (selectedItem == 1 && hasCrowbar)
        {
            selectedItem = 2;
        }
        else if (selectedItem == 2 && hasFlashlight)
        {
            selectedItem = 1;
        }
        else
        {
            return;
        }

        if (selectedItem == 1 && currentBattery <= 0)
        {
            isHoldingItem = false;
        }

        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }

        UpdateEquipmentState();
    }

    public void UpdateEquipmentState()
    {
        StartCoroutine(AnimateItemRoutine());
    }

    private IEnumerator AnimateItemRoutine()
    {
        isChangingItem = true;

        if (characterAnimator != null)
        {
            if (isHoldingItem)
            {
                ApplyEquipmentVisibilityNoInstant();
                if (flashlightInHand != null) flashlightInHand.SetActive(false);
                if (crowbarInHand != null) crowbarInHand.SetActive(false);

                if (selectedItem == 1) characterAnimator.SetTrigger(drawFlashlightTrigger);
                else if (selectedItem == 2) characterAnimator.SetTrigger(drawCrowbarTrigger);

                yield return new WaitForSeconds(drawDelay);
                ApplyEquipmentVisibility();
            }
            else
            {
                if (flashlightLightComponent != null) flashlightLightComponent.enabled = false;
                if (BatteryUI_Flashlight.Instance != null) BatteryUI_Flashlight.Instance.SetVisible(false);

                if (selectedItem == 1) characterAnimator.SetTrigger(hideFlashlightTrigger);
                else if (selectedItem == 2) characterAnimator.SetTrigger(hideCrowbarTrigger);

                yield return new WaitForSeconds(hideDelay);

                ApplyEquipmentVisibility();
            }
        }
        else
        {
            ApplyEquipmentVisibility();
        }

        isChangingItem = false;
    }

    private void ApplyEquipmentVisibilityNoInstant()
    {
        if (flashlightInHand != null) flashlightInHand.SetActive(false);
        if (crowbarInHand != null) crowbarInHand.SetActive(false);
        if (flashlightLightComponent != null) flashlightLightComponent.enabled = false;
        if (BatteryUI_Flashlight.Instance != null) BatteryUI_Flashlight.Instance.SetVisible(false);
    }

    private void ApplyEquipmentVisibility()
    {
        if (flashlightInHand != null) flashlightInHand.SetActive(false);
        if (crowbarInHand != null) crowbarInHand.SetActive(false);
        if (flashlightLightComponent != null) flashlightLightComponent.enabled = false;
        if (BatteryUI_Flashlight.Instance != null) BatteryUI_Flashlight.Instance.SetVisible(false);

        if (!isHoldingItem) return;

        if (selectedItem == 1 && Flashlightpickup.HasFlashlight && flashlightInHand != null)
        {
            flashlightInHand.SetActive(true);
            if (flashlightLightComponent != null) flashlightLightComponent.enabled = isLightOn && (currentBattery > 0);
            if (BatteryUI_Flashlight.Instance != null) BatteryUI_Flashlight.Instance.SetVisible(true);
        }
        else if (selectedItem == 2 && CrowbarPickup.HasCrowbar && crowbarInHand != null)
        {
            crowbarInHand.SetActive(true);
        }
    }

    public void ForceHideItems()
    {
        StopAllCoroutines();
        isHoldingItem = false;
        isChangingItem = false;
        ApplyEquipmentVisibility();
    }
}
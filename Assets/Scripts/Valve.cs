using UnityEngine;
using UnityEngine.Localization;

public class Valve : MonoBehaviour, InterfaceInteractable
{
    [Header("Settings")]
    public string conditionName = "GasVented";
    public string placedCondition = "ValvePlaced";

    [Tooltip("Valve")]
    public GameObject valveMeshToEnable;

    [Tooltip("Wheel")]
    public Transform wheelToRotate;

    [Header("Localization")]
    public LocalizedString textPlaceValve;
    public LocalizedString textTurnValve;
    public LocalizedString textMissingValve;

    [SerializeField] private float rotationAngle = -90f;
    [SerializeField] private float rotationSpeed = 3f;

    [Header("Sound")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip turnSound;

    private bool isPlaced = false;
    private bool isTurned = false;
    private bool isAnimating = false;
    private Quaternion targetRotation;

    private void Start()
    {
        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        if (wheelToRotate != null)
            targetRotation = wheelToRotate.localRotation * Quaternion.Euler(0, 0, rotationAngle);
       

        if (ConditionManager.Instance != null)
        {
            if (!string.IsNullOrEmpty(conditionName) && ConditionManager.Instance.CheckCondition(conditionName))
            {
                isPlaced = true;
                isTurned = true;

                if (wheelToRotate != null)
                    wheelToRotate.localRotation = targetRotation;
                else
                    transform.localRotation = targetRotation;

                if (valveMeshToEnable != null) valveMeshToEnable.SetActive(true);
            }
            else if (!string.IsNullOrEmpty(placedCondition) && ConditionManager.Instance.CheckCondition(placedCondition))
            {
                isPlaced = true;
                isTurned = false;
                if (valveMeshToEnable != null) valveMeshToEnable.SetActive(true);
            }
            else
            {
                if (valveMeshToEnable != null) valveMeshToEnable.SetActive(false);
            }
        }
    }

    private void Update()
    {
        if (isAnimating)
        {
            Transform targetTransform = wheelToRotate != null ? wheelToRotate : transform;

            targetTransform.localRotation = Quaternion.Lerp(targetTransform.localRotation, targetRotation, Time.deltaTime * rotationSpeed);

            if (Quaternion.Angle(targetTransform.localRotation, targetRotation) < 1f)
            {
                targetTransform.localRotation = targetRotation;
                isAnimating = false;

                if (audioSource != null && audioSource.isPlaying)
                {
                    audioSource.Stop();
                }
            }
        }
    }

    public void Interact()
    {
        if (isTurned) return;

        if (!isPlaced)
        {
            if (!ValvePickup.HasValve)
                return;

            isPlaced = true;
            ValvePickup.HasValve = false;

            if (valveMeshToEnable != null)
                valveMeshToEnable.SetActive(true);

            if (UiManager.Instance != null)
                UiManager.Instance.RemoveValveFromUI();

            if (ConditionManager.Instance != null)
                ConditionManager.Instance.SetCondition(placedCondition, true);
        }
        else
        {
            isTurned = true;
            isAnimating = true;

            if (audioSource != null && turnSound != null)
            {
                audioSource.clip = turnSound;
                audioSource.loop = true;
                audioSource.Play();
            }

            if (!string.IsNullOrEmpty(conditionName))
            {
                if (ConditionManager.Instance != null)
                {
                    ConditionManager.Instance.SetCondition(conditionName, true);
                }
            }
        }
    }

    public string GetInteractText()
    {
        if (isTurned)
            return "";

        if (!isPlaced)
        {
            return
                ValvePickup.HasValve ? textPlaceValve.GetLocalizedString() : (textMissingValve != null && !textMissingValve.IsEmpty ? textMissingValve.GetLocalizedString() : "Missing Valve");
        }
        else
        {
            return
                textTurnValve.GetLocalizedString();
        }
    }

    public KeyCode GetInteractKey()
    {
        if (!isPlaced)
            return KeyCode.Mouse0;
        else
            return KeyCode.F;
    }
}
using UnityEngine;
using UnityEngine.Localization;
public class Valve : MonoBehaviour, InterfaceInteractable
{
    [Header("Settings")]
    public string conditionName;

    [Header("Localization")]
    public LocalizedString textTurnValve;

    [SerializeField] private float rotationAngle = -90f;
    [SerializeField] private float rotationSpeed = 3f;

    [Header("Sound")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip turnSound;

    private bool isTurned = false;
    private bool isAnimating = false;
    private Quaternion targetRotation;

    private void Start()
    {
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        targetRotation = transform.localRotation * Quaternion.Euler(0, 0, rotationAngle);
    }

    private void Update()
    {
        if (isAnimating)
        {
            transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * rotationSpeed);

            
            if (Quaternion.Angle(transform.localRotation, targetRotation) < 1f)
            {
                transform.localRotation = targetRotation;
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
                Debug.Log($"Szelep eltekerve: {conditionName}");
            }
        }
    }

    public string GetInteractText()
    {
        if (isTurned)
        {
            return "";
        }
        else
        {
            return textTurnValve.GetLocalizedString();
        }
    }

    public KeyCode GetInteractKey()
    {
        return KeyCode.F;
    }

}
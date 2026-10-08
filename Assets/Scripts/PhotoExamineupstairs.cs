using UnityEngine;
using TMPro;
using System.Collections;

public class PhotoExamineupstairs : MonoBehaviour, InterfaceInteractable
{
    [Header("Camera System")]
    public Camera mainPlayerCamera;
    public Camera photoExamineCamera;

    [Header("Examine Settings")]
    public Transform examinePoint;
    public float rotationSpeed = 5f;
    public KeyCode interactKey = KeyCode.F;

    public Vector3 examineScale = new Vector3(1.0f, 1.0f, 1.0f);
    public float requiredAngleToTrigger = 160f;
    public float autoExitDelay = 3.5f;

    [Header("Visual Settings")]
    public GameObject examineLight;

    [Header("UI and HUD")]
    public GameObject examineUI;
    public TextMeshProUGUI hintText;
    public GameObject[] hudElementsToHide;

    [Header("Story Logic")]
    public string requiredConditionToSet = "SawBedroomClue";
    public AudioSource radioToTurnOff;

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Vector3 originalScale;

    private bool isExamining = false;
    private bool clueDiscovered = false;

    [Header("Player Control")]
    public OwnMovement playerController;
    public Interaction playerInteraction;

    void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        originalScale = transform.localScale;

        if (examineUI != null) examineUI.SetActive(false);
        if (examineLight != null) examineLight.SetActive(false);
        if (photoExamineCamera != null) photoExamineCamera.gameObject.SetActive(false);

        StartCoroutine(CheckStatusAfterLoad());
    }

    private IEnumerator CheckStatusAfterLoad()
    {
        yield return new WaitForSeconds(0.1f);

        if (ConditionManager.Instance != null && ConditionManager.Instance.CheckCondition(requiredConditionToSet))
        {
            clueDiscovered = true;

            if (radioToTurnOff != null)
            {
                radioToTurnOff.Stop();
            }
        }
    }

    void Update()
    {
        if (isExamining)
        {
            float rotX = Input.GetAxis("Mouse X") * rotationSpeed;
            float rotY = Input.GetAxis("Mouse Y") * rotationSpeed;

            transform.Rotate(photoExamineCamera.transform.up, -rotX, Space.World);
            transform.Rotate(photoExamineCamera.transform.right, rotY, Space.World);

            float angle = Vector3.Angle(transform.forward, photoExamineCamera.transform.forward);

            if (angle > requiredAngleToTrigger && !clueDiscovered)
            {
                clueDiscovered = true;

                if (ConditionManager.Instance != null)
                {
                    ConditionManager.Instance.SetCondition(requiredConditionToSet, true);
                }

                if (hintText != null)
                {
                    hintText.text = "You found the clue!";
                }

                StartCoroutine(AutoExitRoutine());
            }

            if (Input.GetKeyDown(KeyCode.Escape) && !clueDiscovered)
            {
                StopExamining();
            }
        }
    }

    private IEnumerator AutoExitRoutine()
    {
        yield return new WaitForSeconds(autoExitDelay);
        StopExamining();
    }

    public void Interact()
    {
        if (clueDiscovered) return;
        if (ConditionManager.Instance != null && ConditionManager.Instance.CheckCondition(requiredConditionToSet)) return;

        if (!isExamining) StartExamining();
    }

    private void StartExamining()
    {
        if (radioToTurnOff != null)
        {
            radioToTurnOff.Stop();
        }

        isExamining = true;

        if (playerController != null) playerController.enabled = false;
        if (playerInteraction != null) playerInteraction.enabled = false;

        foreach (GameObject hudItem in hudElementsToHide)
        {
            if (hudItem != null) hudItem.SetActive(false);
        }

        if (mainPlayerCamera != null) mainPlayerCamera.gameObject.SetActive(false);
        if (photoExamineCamera != null) photoExamineCamera.gameObject.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        transform.position = examinePoint.position;
        transform.LookAt(photoExamineCamera.transform);
        transform.Rotate(0, 180, 0);
        transform.localScale = examineScale;

        if (examineLight != null) examineLight.SetActive(true);
        if (examineUI != null) examineUI.SetActive(true);

        if (hintText != null && !clueDiscovered)
            hintText.text = "Rotate (Mouse)\nExit (Esc)";
    }

    private void StopExamining()
    {
        isExamining = false;

        if (playerController != null) playerController.enabled = true;
        if (playerInteraction != null) playerInteraction.enabled = true;

        foreach (GameObject hudItem in hudElementsToHide)
        {
            if (hudItem != null) hudItem.SetActive(true);
        }

        if (photoExamineCamera != null) photoExamineCamera.gameObject.SetActive(false);
        if (mainPlayerCamera != null) mainPlayerCamera.gameObject.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        transform.position = originalPosition;
        transform.rotation = originalRotation;
        transform.localScale = originalScale;

        if (examineLight != null) examineLight.SetActive(false);
        if (examineUI != null) examineUI.SetActive(false);
    }

    public string GetInteractText()
    {
        bool alreadyFound = clueDiscovered || (ConditionManager.Instance != null && ConditionManager.Instance.CheckCondition(requiredConditionToSet));
        if (alreadyFound) return "";

        return "Examine the photo";
    }

    public KeyCode GetInteractKey() => interactKey;
}
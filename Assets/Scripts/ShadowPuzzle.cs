using UnityEngine;
using System.Collections.Generic;

public class ShadowPuzzle : MonoBehaviour, InterfaceInteractable
{
    [Header("Projector")]
    public Projector_ShadowPuzzle projector;

    [Header("Puzzle Settings")]
    public float marginOfError = 5f;
    public float sensitivity = 100f;

    [System.Serializable]
    public struct HiddenNumber
    {
        public GameObject numberObject; 
        public float targetYAngle; 
    }

    [Header("Hidden Numbers")]
    public List<HiddenNumber> hiddenNumbers;

    [Header("Camera and Player")]
    public GameObject puzzleCameraObj;
    public GameObject playerObj;
    public GameObject playerCameraObj;
    public GameObject playerVisuals;

    private bool puzzleActive = false;

    void Start()
    {
        if (puzzleCameraObj != null) puzzleCameraObj.SetActive(false);

        if (hiddenNumbers != null)
        {
            foreach (var item in hiddenNumbers)
            {
                if (item.numberObject != null) item.numberObject.SetActive(false);
            }
        }
    }

    public string GetInteractText()
    {
        if (puzzleActive) return "";

        if (projector != null && projector.isTurnedOn)
        {
            return "Use Projector";
        }
        return "";
    }

    public KeyCode GetInteractKey() => KeyCode.F;

    public void Interact()
    {
        if (projector != null && projector.isTurnedOn && !puzzleActive)
        {
            EnterPuzzle();
        }
    }

    void Update()
    {
        if (!puzzleActive) return;

        if (Input.GetMouseButton(0))
        {
            float rotX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
            transform.Rotate(Vector3.up, -rotX, Space.World);

            CheckScannedNumbers();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitPuzzle();
        }
    }

    void EnterPuzzle()
    {
        if (playerVisuals != null) playerVisuals.SetActive(false);
        puzzleActive = true;

        if (playerCameraObj != null) playerCameraObj.SetActive(false);
        if (puzzleCameraObj != null) puzzleCameraObj.SetActive(true);

        if (playerObj != null)
        {
            var controller = playerObj.GetComponent<MonoBehaviour>();
            if (controller != null) controller.enabled = false;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ExitPuzzle()
    {
        if (playerVisuals != null) playerVisuals.SetActive(true);
        puzzleActive = false;

        if (puzzleCameraObj != null) puzzleCameraObj.SetActive(false);
        if (playerCameraObj != null) playerCameraObj.SetActive(true);

        if (playerObj != null)
        {
            var controller = playerObj.GetComponent<MonoBehaviour>();
            if (controller != null) controller.enabled = true;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void CheckScannedNumbers()
    {
        float currentY = transform.eulerAngles.y;

        if (hiddenNumbers != null)
        {
            foreach (var item in hiddenNumbers)
            {
                if (item.numberObject == null) continue;

                float angleDiff = Mathf.Abs(Mathf.DeltaAngle(currentY, item.targetYAngle));

                if (angleDiff < marginOfError)
                {
                    if (!item.numberObject.activeSelf)
                    {
                        item.numberObject.SetActive(true);
                    }
                }
                else
                {
                    if (item.numberObject.activeSelf)
                    {
                        item.numberObject.SetActive(false);
                    }
                }
            }
        }
    }
}
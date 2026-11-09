using System.Collections;
using UnityEngine;

public class Sink_Interactable : MonoBehaviour, InterfaceInteractable
{
    [Header("Puzzle Manager")]
    public ClassroomPuzzleManager puzzleManager;

    [Header("References")]
    public Transform handle;
    public ParticleSystem waterParticles;
    public AudioSource waterAudio;

    [Header("Water Level Settings")]
    public Transform waterLevelObject;
    public float fillSpeed = 0.05f;
    public float maxWaterHeight = 0.5f;
    public float minWaterHeight = 0.1f;

    [Header("Settings")]
    public Vector3 openRotation = new Vector3(0, 90, 0);
    public float rotationSpeed = 3.0f;
    public KeyCode interactKey = KeyCode.F;

    private Vector3 closedRotation;
    public bool isOpen = false;
    private bool isAnimating = false;
    public bool isFilled = false;

    private Coroutine fillWaterCoroutine;

    void Start()
    {
        if (handle != null) closedRotation = handle.localEulerAngles;

        isOpen = SaveSystem.IsSinkOpen;
        isFilled = SaveSystem.IsSinkFilled;

        float startHeight;

        if (SaveSystem.SinkWaterLevel > 0)
        {
            startHeight = SaveSystem.SinkWaterLevel;
        }
        else
        {
            startHeight = minWaterHeight;
        }

        if (isFilled)
        {
            isOpen = false;
            handle.localEulerAngles = closedRotation;
            if (waterLevelObject != null)
            {
                waterLevelObject.gameObject.SetActive(true);
                SetWaterY(maxWaterHeight);
            }
        }
        else
        {
            if (isOpen)
            {
                handle.localEulerAngles = openRotation;
                if (waterParticles != null) waterParticles.Play();
                if (waterAudio != null) waterAudio.Play();
                fillWaterCoroutine = StartCoroutine(FillWaterRoutine(startHeight));
            }
            else
            {
                handle.localEulerAngles = closedRotation;
            }

            if (waterLevelObject != null)
            {
                if (startHeight > minWaterHeight)
                {
                    waterLevelObject.gameObject.SetActive(true);
                    SetWaterY(startHeight);
                }
                else
                {
                    waterLevelObject.gameObject.SetActive(false);
                    SetWaterY(minWaterHeight);
                }
            }
        }
    }

    public string GetInteractText()
    {
        if (puzzleManager != null && puzzleManager.hasReadRiddle == false)
        {
            return "";
        }

        if (isFilled == true)
        {
            return "";
        }

        if (isOpen == true)
        {
            return "Close Tap";
        }
        else
        {
            return "Turn On Water";
        }
    }

    public KeyCode GetInteractKey()
    {
        return interactKey;
    }

    public void Interact()
    {
        if (isAnimating || isFilled) return;

        if (puzzleManager != null)
        {
            bool isSuccess = puzzleManager.InteractWithSink(!isOpen);

            if (!isSuccess) return;
        }

        StartCoroutine(AnimateTapRoutine());
    }

    private IEnumerator AnimateTapRoutine()
    {
        isAnimating = true;

        if (isOpen == true)
        {
            isOpen = false;
        }
        else
        {
            isOpen = true;
        }

        SaveSystem.IsSinkOpen = isOpen;

        Quaternion startRot = handle.localRotation;
        Quaternion endRot;

        if (isOpen == true)
        {
            endRot = Quaternion.Euler(openRotation);
        }
        else
        {
            endRot = Quaternion.Euler(closedRotation);
        }

        float time = 0;

        while (time < 1)
        {
            time += Time.deltaTime * rotationSpeed;
            handle.localRotation = Quaternion.Lerp(startRot, endRot, time);
            yield return null;
        }

        handle.localRotation = endRot;

        if (isOpen == true)
        {
            if (waterParticles != null)
            {
                waterParticles.Play();
            }

            if (waterAudio != null)
            {
                waterAudio.Play();
            }

            if (fillWaterCoroutine != null)
            {
                StopCoroutine(fillWaterCoroutine);
            }

            float currentY;
            if (waterLevelObject != null)
            {
                currentY = waterLevelObject.localPosition.y;
            }
            else
            {
                currentY = minWaterHeight;
            }

            fillWaterCoroutine = StartCoroutine(FillWaterRoutine(currentY));
        }
        else
        {
            if (waterParticles != null)
            {
                waterParticles.Stop();
            }

            if (waterAudio != null)
            {
                waterAudio.Stop();
            }

            if (fillWaterCoroutine != null)
            {
                StopCoroutine(fillWaterCoroutine);
            }
        }

        isAnimating = false;
    }

    public void ResetSink()
    {
        isOpen = false;
        isFilled = false;
        SaveSystem.IsSinkOpen = false;
        SaveSystem.IsSinkFilled = false;

        if (waterParticles != null) waterParticles.Stop();
        if (waterAudio != null) waterAudio.Stop();
        if (fillWaterCoroutine != null) StopCoroutine(fillWaterCoroutine);

        handle.localRotation = Quaternion.Euler(closedRotation);
        SetWaterY(minWaterHeight);
        if (waterLevelObject != null) waterLevelObject.gameObject.SetActive(false);

        isAnimating = false;
    }

    private IEnumerator FillWaterRoutine(float startHeight)
    {
        if (waterLevelObject == null) yield break;

        waterLevelObject.gameObject.SetActive(true);
        float currentHeight = startHeight;

        while (currentHeight < maxWaterHeight)
        {
            currentHeight += Time.deltaTime * fillSpeed;
            SetWaterY(currentHeight);
            SaveSystem.SinkWaterLevel = currentHeight;
            yield return null;
        }

        isFilled = true;
        SaveSystem.IsSinkFilled = true;
        StartCoroutine(AutoCloseRoutine());
    }

    private IEnumerator AutoCloseRoutine()
    {
        isAnimating = true;
        isOpen = false;
        SaveSystem.IsSinkOpen = false;

        if (waterParticles != null) waterParticles.Stop();
        if (waterAudio != null) waterAudio.Stop();

        Quaternion startRot = handle.localRotation;
        Quaternion endRot = Quaternion.Euler(closedRotation);
        float time = 0;

        while (time < 1)
        {
            time += Time.deltaTime * rotationSpeed;
            handle.localRotation = Quaternion.Lerp(startRot, endRot, time);
            yield return null;
        }

        handle.localRotation = endRot;
        isAnimating = false;
    }

    private void SetWaterY(float posY)
    {
        if (waterLevelObject != null)
        {
            Vector3 pos = waterLevelObject.localPosition;
            waterLevelObject.localPosition = new Vector3(pos.x, posY, pos.z);
        }
    }
}
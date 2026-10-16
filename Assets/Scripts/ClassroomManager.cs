using System.Collections;
using UnityEngine;
using TMPro;

public class ClassroomPuzzleManager : MonoBehaviour
{
    [Header("Puzzle Activation")]
    public bool hasReadRiddle = false;

    [Header("Current step")]
    public int currentStep = 0;

    [Header("Item references")]
    public Sink_Interactable sinkScript;
    public TextMeshPro textOnBoard;
    public float eraseDuration = 1.5f;

    public UniversalDoor puzzleDoor;
    public GameObject padlockKeyObject;

    [Header("Chair Physics Reset")]
    public Transform chairTransform;
    private Vector3 chairStartPos;
    private Quaternion chairStartRot;

    [Header("Player HP & Punishment")]
    public HP playerHP;
    public float damageOnFail = 20f;

    [Header("Sounds")]
    public AudioSource mainAudioSource;
    public AudioClip failBuzzerSound;
    public AudioClip successVoiceSound;

    [Header("Board Audio")]
    public AudioSource boardAudioSource;
    public AudioClip wipeLoopSound;

    private Coroutine eraseCoroutine;

    private bool isResetting = false;

    private void Start()
    {
        if (chairTransform != null)
        {
            chairStartPos = chairTransform.position;
            chairStartRot = chairTransform.rotation;
        }

        if (!hasReadRiddle)
        {
            if (textOnBoard != null)
            {
                textOnBoard.gameObject.SetActive(false);
            }
        }
    }

    public void UnlockPuzzleObjects()
    {
        if (!hasReadRiddle)
        {
            hasReadRiddle = true;

            if (textOnBoard != null)
            {
                textOnBoard.gameObject.SetActive(true);
            }
        }
    }

    public bool InteractWithSink(bool isOpening)
    {
        if (!hasReadRiddle) return false;

        if (isOpening)
        {
            if (currentStep == 0)
            {
                currentStep = 1;
                return true;
            }
            else
            {
                FailPuzzle();
                return false;
            }
        }
        else
        {
            if (currentStep < 4)
            {
                FailPuzzle();
                return false;
            }
            return true;
        }
    }

    public void InteractWithBoard()
    {
        if (!hasReadRiddle) return;
        if (currentStep >= 2) return;

        if (currentStep == 1)
        {
            currentStep = 2;
            eraseCoroutine = StartCoroutine(EraseTextRoutine());
        }
        else
        {
            FailPuzzle();
        }
    }

    private IEnumerator EraseTextRoutine()
    {
        if (textOnBoard == null) yield break;

        if (boardAudioSource != null && wipeLoopSound != null)
        {
            boardAudioSource.clip = wipeLoopSound;
            boardAudioSource.loop = true;
            boardAudioSource.Play();
        }

        Color startColor = textOnBoard.color;
        float timeElapsed = 0;

        while (timeElapsed < eraseDuration)
        {
            timeElapsed += Time.deltaTime;
            float newAlpha = Mathf.Lerp(1f, 0f, timeElapsed / eraseDuration);
            textOnBoard.color = new Color(startColor.r, startColor.g, startColor.b, newAlpha);
            yield return null;
        }

        textOnBoard.color = new Color(startColor.r, startColor.g, startColor.b, 0f);

        if (boardAudioSource != null)
        {
            boardAudioSource.Stop();
        }
    }

    public bool InteractWithDoor(bool isOpening)
    {
        if (isResetting) return true;

        if (!hasReadRiddle) return true;

        if (currentStep >= 4) return true;

        if (isOpening)
        {
            if (currentStep > 0 && currentStep < 4)
            {
                FailPuzzle();
                return false;
            }
            return true;
        }
        else
        {
            if (currentStep >= 3) return true;

            if (currentStep == 2)
            {
                currentStep = 3;
                return true;
            }
            else
            {
                FailPuzzle();
                return false;
            }
        }
    }

    public void InteractWithDesk()
    {
        if (!hasReadRiddle) return;
        if (currentStep >= 4) return;

        if (currentStep == 3)
        {
            currentStep = 4;
            mainAudioSource.PlayOneShot(successVoiceSound);

            if (padlockKeyObject != null)
            {
                padlockKeyObject.SetActive(true);
            }

            if (puzzleDoor != null)
            {
                puzzleDoor.UnlockOnly();
            }

            if (chairTransform != null)
            {
                Rigidbody rb = chairTransform.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = true;
                }

                PushInteractable pushScript = chairTransform.GetComponent<PushInteractable>();
                if (pushScript != null)
                {
                    pushScript.enabled = false;
                }
            }
        }
        else
        {
            FailPuzzle();
        }
    }

    private void FailPuzzle()
    {
        if (isResetting) return;
        isResetting = true;

        mainAudioSource.PlayOneShot(failBuzzerSound);

        if (sinkScript != null)
        {
            sinkScript.ResetSink();
        }

        if (puzzleDoor != null && !puzzleDoor.IsOpen())
        {
            puzzleDoor.ForceOpen();
        }

        if (eraseCoroutine != null)
        {
            StopCoroutine(eraseCoroutine);
        }

        if (boardAudioSource != null)
        {
            boardAudioSource.Stop();
        }

        if (textOnBoard != null)
        {
            Color c = textOnBoard.color;
            c.a = 1f;
            textOnBoard.color = c;
        }

        if (chairTransform != null)
        {
            Rigidbody rb = chairTransform.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            chairTransform.position = chairStartPos;
            chairTransform.rotation = chairStartRot;
        }

        if (playerHP != null)
        {
            playerHP.TakeDamage(damageOnFail);
        }

        currentStep = 0;

        isResetting = false;
    }
}
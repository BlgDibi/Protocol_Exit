using System.Collections;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings; 
using System;

public class Talk : MonoBehaviour, InterfaceInteractable
{
    [Header("Transforms")]
    public Transform playerTrans;
    public Transform speakerTrans;
    public Transform speakerLook;

    private Quaternion initialRotation;

    [Header("Localization - Interaction")]
    public LocalizedString textTalkPrompt;

    [Header("Dialogue")]
    public AudioClip dialogueClip;
    [TextArea(3, 10)]
    public string dialogueText;
    public AudioSource dialogueSource;

    [Header("UI - Subtitles")]
    public TMPro.TextMeshProUGUI subtitleText;

    [Header("Player")]
    public StarterAssets.ThirdPersonController playerController;

    [Header("Door Message")]
    public TMPro.TextMeshProUGUI infoText;
    public string doorColor;
    public float messageDuration = 3f;

    public event Action OnDialogueFinished;
    public bool HasTalked = false;

    [SerializeField] private KeyCode interactkey=KeyCode.T;

    private void Start()
    {
        if (speakerTrans != null)
            initialRotation = speakerTrans.rotation;

        OnDialogueFinished += ShowDoorMessage;

        if (subtitleText != null) subtitleText.gameObject.SetActive(false);
        if (infoText != null) infoText.gameObject.SetActive(false);
    }


    public KeyCode GetInteractKey()
    {
        return KeyCode.T;
    }

    public string GetInteractText()
    {
        return textTalkPrompt.GetLocalizedString();
    }

    public void Interact()
    {
        StartDialogue();
    }


    public void StartDialogue()
    {
        if (speakerTrans != null && playerTrans != null)
        {
            Vector3 targetPostition = new Vector3(playerTrans.position.x, speakerTrans.position.y, playerTrans.position.z);
            speakerTrans.LookAt(targetPostition);
        }

        if (playerController != null)
        {
            playerController.MoveSpeed = 0f;
            playerController.canMove = false; 
        }

        if (subtitleText != null)
        {
            subtitleText.text = dialogueText;
            subtitleText.gameObject.SetActive(true);
        }

        float duration = 5f;
        if (dialogueClip != null && dialogueSource != null)
        {
            dialogueSource.clip = dialogueClip;
            dialogueSource.Play();
            duration = dialogueClip.length;
        }

        HasTalked = true;
        StartCoroutine(HideTextAfterDelay(duration));
    }

    private IEnumerator HideTextAfterDelay(float duration)
    {
        yield return new WaitForSeconds(duration);

        if (subtitleText != null) subtitleText.gameObject.SetActive(false);

        if (speakerTrans != null) speakerTrans.rotation = initialRotation;

        if (playerController != null)
        {
           playerController.canMove = true;
        }

        OnDialogueFinished?.Invoke();
    }


    private void ShowDoorMessage()
    {
        if (infoText != null && !string.IsNullOrEmpty(doorColor))
        {
            StartCoroutine(DisplayDoorMessageCoroutine());
        }
    }

    private IEnumerator DisplayDoorMessageCoroutine()
    {
        var currentLocale = LocalizationSettings.SelectedLocale;
        bool magyarNyelv = false;

        if (currentLocale != null)
        {
            magyarNyelv = currentLocale.Identifier.Code.Contains("hu");
        }

        string message = "";

        if (magyarNyelv)
        {
            string magyarSzin = doorColor;
            if (doorColor.ToLower() == "green") magyarSzin = "zöld";
            else if (doorColor.ToLower() == "red") magyarSzin = "piros";
            else if (doorColor.ToLower() == "blue") magyarSzin = "kék";

            message = $"Egy {magyarSzin} ajtó megjelent valahol!";
        }
        else
        {
            message = $"A {doorColor} door appeared somewhere!";
        }

        infoText.text = message;
        infoText.gameObject.SetActive(true);

        yield return new WaitForSeconds(messageDuration);

        infoText.gameObject.SetActive(false);
    }

    public KeyCode GetInteracKey()
    {
        return interactkey;
    }

}
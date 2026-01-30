using Ink.Runtime;
using StarterAssets;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Header("Ink File")]
    [SerializeField] private TextAsset inkJSONAsset;

    [Header("UI Items")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("Answer Buttons")]
    [SerializeField] private GameObject choiceButtonPrefab;
    [SerializeField] private Transform choicesContainer;

    [Header("Player References")]
    [SerializeField] private ThirdPersonController playerController;
    [SerializeField] private StarterAssetsInputs playerInputs;

    private Story currentStory;
    public bool isDialoguePlaying { get; private set; }

    private void Start()
    {
        isDialoguePlaying = false;
        dialoguePanel.SetActive(false);

        if (GameEvents.instance != null && GameEvents.instance.dialogueEvents != null)
        {
            GameEvents.instance.dialogueEvents.onEnterDialogue += EnterDialogueMode;
        }
    }

    private void Update()
    {
        if (!isDialoguePlaying) return;

        if (currentStory.currentChoices.Count == 0)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.F))
            {
                ContinueStory();
            }
        }
    }

    public void EnterDialogueMode(string knotName)
    {
        if (isDialoguePlaying) return;

        
        if (currentStory == null)
        {
            currentStory = new Story(inkJSONAsset.text);
        }

        currentStory.ChoosePathString(knotName);
        isDialoguePlaying = true;
        dialoguePanel.SetActive(true);

        SetPlayerEnabled(false);
        ContinueStory();
    }

    public void ContinueStory()
    {
        if (currentStory.canContinue)
        {
            string nextLine = currentStory.Continue();

            HandleTags(currentStory.currentTags);

            if (string.IsNullOrWhiteSpace(nextLine))
            {
                ContinueStory();
            }
            else
            {
                dialogueText.text = nextLine;
                DisplayChoices();
            }
        }
        else if (currentStory.currentChoices.Count > 0)
        {
            DisplayChoices();
        }
        else
        {
            ExitDialogueMode();
        }
    }

    private void HandleTags(List<string> tags)
    {
        foreach (string tag in tags)
        {
            if (tag == "move_book")
            {
                SecretRevealer revealer = FindObjectOfType<SecretRevealer>();
                if (revealer != null)
                {
                    revealer.ActivateSecret();
                }
            }
        }
    }

    private void DisplayChoices()
    {
        foreach (Transform child in choicesContainer) Destroy(child.gameObject);

        foreach (Choice choice in currentStory.currentChoices)
        {
            GameObject choiceButton = Instantiate(choiceButtonPrefab, choicesContainer);
            choiceButton.transform.localScale = Vector3.one;

            DialogueChoiceButton buttonScript = choiceButton.GetComponent<DialogueChoiceButton>();

            if (buttonScript != null)
            {
                buttonScript.Setup(choice, this);
            }
            else
            {
                TextMeshProUGUI text = choiceButton.GetComponentInChildren<TextMeshProUGUI>();
                if (text != null) text.text = choice.text;

                choiceButton.GetComponent<Button>().onClick.AddListener(() => MakeChoice(choice.index));
            }
        }
    }

    public void MakeChoice(int choiceIndex)
    {
        currentStory.ChooseChoiceIndex(choiceIndex);
        ContinueStory();
    }

    public void ExitDialogueMode()
    {
        isDialoguePlaying = false; 
        dialoguePanel.SetActive(false);
        dialogueText.text = "";

        foreach (Transform child in choicesContainer)
        {
            Destroy(child.gameObject);
        }

        SetPlayerEnabled(true);
    }

    private void SetPlayerEnabled(bool enabled)
    {
        if (playerInputs != null)
        {
            playerInputs.enabled = enabled;
            if (!enabled)
            {
                playerInputs.move = Vector2.zero;
                playerInputs.look = Vector2.zero;
            }
        }

        if (playerController != null)
        {
            playerController.freezeAll = !enabled;
            playerController.enabled = enabled;
        }

        Cursor.visible = !enabled;
        Cursor.lockState = enabled ? CursorLockMode.Locked : CursorLockMode.None;
    }

    private bool IsLineBlank(string dialogueLine)
    {
        return string.IsNullOrWhiteSpace(dialogueLine) || dialogueLine.Trim().Equals("\n");
    }
}
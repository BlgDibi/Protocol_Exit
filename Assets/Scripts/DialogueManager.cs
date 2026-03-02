using Ink.Runtime;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

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

    [Header("Typing Settings")]
    [SerializeField] private float typingSpeed = 0.04f;
    private Coroutine typingCoroutine;

    [Header("Rewards")]
    [SerializeField] private GameObject greenKey;


    private static Dictionary<string, string> inkMemory = new Dictionary<string, string>();
    private TextAsset currentInkAsset;


    private Story currentStory;
    public bool isDialoguePlaying { get; private set; }

    private Talk currentNPC;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }
    }

    private void Start()
    {
        isDialoguePlaying = false;
        dialoguePanel.SetActive(false);

       
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

    public void EnterDialogueMode(TextAsset specificInkFile, string knotName = "", Talk npc = null)
    {
        if (isDialoguePlaying) return;

        currentNPC = npc;
        currentInkAsset = specificInkFile;

        currentStory = new Story(specificInkFile.text);

        currentStory.BindExternalFunction("is_scale_solved", () =>
        {
            WeightPlate Scale = FindFirstObjectByType<WeightPlate>();
            if (Scale != null) return Scale.IsWeightOptimal();
            return false;
        });

        if (inkMemory.ContainsKey(specificInkFile.name))
        {
            currentStory.state.LoadJson(inkMemory[specificInkFile.name]);
        }

        if (!string.IsNullOrEmpty(knotName))
        {
            currentStory.ChoosePathString(knotName);
        }

        isDialoguePlaying = true;
        dialoguePanel.SetActive(true);
        SetPlayerEnabled(false);
        ContinueStory();
    }

    public void ContinueStory()
    {
        if (currentStory.canContinue)
        {
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);

            string nextLine = currentStory.Continue();
            HandleTags(currentStory.currentTags);

            if (string.IsNullOrWhiteSpace(nextLine))
            {
                ContinueStory();
            }
            else
            {
                typingCoroutine = StartCoroutine(TypeText(nextLine));
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
                SecretRevealer revealer = FindFirstObjectByType<SecretRevealer>();
                if (revealer != null) revealer.ActivateSecret();
            }
            else if (tag == "spawn_green_key")
            {
                if (greenKey != null)
                {
                    greenKey.SetActive(true); 
                    Debug.Log("Green key showed somewhere!");
                }
            }
            else if (tag.StartsWith("play_audio"))
            {
                if (currentNPC != null) currentNPC.PlayVoiceLine(tag);
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

        Canvas.ForceUpdateCanvases();
    }

    public void MakeChoice(int choiceIndex)
    {
        currentStory.ChooseChoiceIndex(choiceIndex);
        ContinueStory();
    }

    public void ExitDialogueMode()
    {
        if (currentStory != null && currentInkAsset != null)
        {
            inkMemory[currentInkAsset.name] = currentStory.state.ToJson();
        }

        isDialoguePlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";

        foreach (Transform child in choicesContainer) Destroy(child.gameObject);

        SetPlayerEnabled(true);

        if (currentNPC != null)
        {
            currentNPC.ResetRotation();
            currentNPC = null;
        }
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

    private IEnumerator TypeText(string text)
    {
        dialogueText.text = "";
        foreach (Transform child in choicesContainer) child.gameObject.SetActive(false);

        foreach (char letter in text.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        DisplayChoices();
    }

  
}
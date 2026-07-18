using Ink.Runtime;
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
    [SerializeField] private OwnMovement playerController;

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

    public static void SetInkMemory(Dictionary<string, string> loadedMemory)
    {
        inkMemory = loadedMemory;
    }

    public static Dictionary<string, string> GetInkMemory()
    {
        return inkMemory;
    }

    private void Start()
    {
        isDialoguePlaying = false;
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
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
            if (ConditionManager.Instance != null)
                return ConditionManager.Instance.CheckCondition("ScaleSolved");

            return false;
        });

        currentStory.BindExternalFunction("check_condition", (string conditionName) =>
        {
            if (ConditionManager.Instance != null)
                return ConditionManager.Instance.CheckCondition(conditionName);

            return false;
        });

        if (inkMemory.ContainsKey(specificInkFile.name))
        {
            currentStory.state.LoadJson(inkMemory[specificInkFile.name]);
        }

        if (!string.IsNullOrEmpty(knotName))
        {
            try
            {
                currentStory.ChoosePathString(knotName);
            }
            catch (System.Exception e)
            {
                Debug.LogError("There is no Knot in Ink file: " + knotName + " | Error: " + e.Message);
            }
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
                SpawnGreenKey();
            }
            else if (tag.StartsWith("voice:"))
            {
                if (currentNPC != null)
                {
                    string fileName = tag.Replace("voice:", "").Trim();
                    Debug.Log("Dialogumenager voice: " + fileName);
                    currentNPC.PlayVoiceLine(fileName);
                }
            }
        }
    }

    public void ExitDialogueMode()
    {
        if (currentStory != null && currentInkAsset != null)
        {
            inkMemory[currentInkAsset.name] = currentStory.state.ToJson();
            Debug.Log("Ink memory saved: " + currentInkAsset.name);
        }

        if (currentNPC != null)
        {
            DialogueTrigger trigger = currentNPC.GetComponent<DialogueTrigger>();
            if (trigger != null) trigger.DialogueEnded();
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

    public string GetVariableState(string variableName)
    {
        if (currentStory != null)
        {
            try
            {
                var value = currentStory.variablesState[variableName];
                if (value != null) return value.ToString();
            }
            catch { }
        }
        return "";
    }

    private void DisplayChoices()
    {
        foreach (Transform child in choicesContainer) Destroy(child.gameObject);

        foreach (Choice choice in currentStory.currentChoices)
        {
            GameObject choiceButton = Instantiate(choiceButtonPrefab, choicesContainer);
            choiceButton.transform.localScale = Vector3.one;

            TextMeshProUGUI text = choiceButton.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null) text.text = choice.text;

            choiceButton.GetComponent<Button>().onClick.AddListener(() => MakeChoice(choice.index));
        }
        Canvas.ForceUpdateCanvases();
    }

    public void MakeChoice(int choiceIndex)
    {
        currentStory.ChooseChoiceIndex(choiceIndex);
        ContinueStory();
    }

    private void SetPlayerEnabled(bool enabled)
    {
        if (playerController != null)
        {
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

        if (currentStory.currentChoices.Count > 0)
        {
            DisplayChoices();
        }
    }

    public void SpawnGreenKey()
    {
        if (greenKey != null)
        {
            PersistentItem pi = greenKey.GetComponent<PersistentItem>();
            if (pi != null)
            {
                pi.SetVisibility(true);
            }

            SaveSystem.SaveNPCMet("green_key_spawned");
            Debug.Log("<color=yellow>Key Spawned!</color>");
        }
    }
}
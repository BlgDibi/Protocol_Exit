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

        StartCoroutine(CheckAndSpawnKeyOnLoad());
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

    public static void SetInkMemory(Dictionary<string, string> loadedMemory)
    {
        inkMemory = loadedMemory;
    }

    public static Dictionary<string, string> GetInkMemory()
    {
        return inkMemory;
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
                SpawnGreenKey();
            }
            else if (tag.StartsWith("voice:"))
            {
                if (currentNPC != null)
                {
                    string voiceFile = tag.Replace("voice:", "").Trim();
                    currentNPC.PlayVoiceLine(voiceFile);
                }
            }
            else if (tag == "play_audio")
            {
                if (currentNPC != null) currentNPC.PlayVoiceLine("default_audio");
            }
        }
    }
    private IEnumerator CheckAndSpawnKeyOnLoad()
    {
        // Várunk a SaveSystem-re
        yield return new WaitForSeconds(0.8f);

        string state = GetVariableState("got_reward");
        Debug.Log("Manager betöltéskor: got_reward = " + state);

        if (state.ToLower() == "true")
        {
            // Elõször ellenõrizzük az ID-t (3-as ID a zöld kulcs)
            List<int> invKeys = SaveSystem.GetInventoryKeys();

            if (invKeys == null || !invKeys.Contains(3))
            {
                Debug.Log("Manager: A kulcs még nincs az inventory-ban, aktiválom a tárgyat!");
                SpawnGreenKey(); // Ez teszi aktívvá (SetActive(true))
            }
            else
            {
                Debug.Log("Manager: A kulcs már nálad van, nem spawnolom újra.");
            }
        }
    }

    public void SpawnGreenKey()
    {
        if (greenKey != null)
        {
            greenKey.SetActive(true);
            var rend = greenKey.GetComponentInChildren<Renderer>();
            if (rend != null) rend.enabled = true;
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

        foreach (var entry in inkMemory)
        {
            string fileName = entry.Key; // Ez nálad most "npc1"
            TextAsset asset = null;

            // 1. Próbálkozás: Simán a Resources mappában
            asset = Resources.Load<TextAsset>(fileName);

            // 2. Próbálkozás: Az Ink Resources mappában
            if (asset == null) asset = Resources.Load<TextAsset>("ink resources/" + fileName);

            // 3. Próbálkozás: Kisbetûs/Nagybetûs variációk kizárása (opcionális, de segít)
            if (asset == null) asset = Resources.Load<TextAsset>(fileName.ToLower());

            if (asset == null)
            {
                // Ha még mindig nem leli, kiírjuk a pontos elérési utat, amit próbált
                Debug.LogError($"HIBA: Nem lelem a fájlt! Keresett név: '{fileName}' vagy 'Ink Resources/{fileName}'. Ellenõrizd a Resources mappádat!");
                continue;
            }

            try
            {
                Story tempStory = new Story(asset.text);
                tempStory.state.LoadJson(entry.Value);
                var value = tempStory.variablesState[variableName];
                if (value != null) return value.ToString();
            }
            catch (System.Exception e)
            {
                Debug.LogError("Hiba a JSON betöltésekor: " + e.Message);
            }
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
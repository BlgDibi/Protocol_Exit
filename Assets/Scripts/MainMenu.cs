using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject timeMenuPanel;
    public GameObject loadMenuPanel;
    public GameObject confirmationPanel;
    public GameObject mainMenuPanel;
    public GameObject customizerPanel;

    [Header("Character")]
    public GameObject mainCharacter;
    public GameObject dummyCharacter;

    [Header("UI Text")]
    public TextMeshProUGUI warningText;

    [Header("Slot Texts")]
    public TextMeshProUGUI[] loadSlotTexts;

    private void Start()
    {
        if (confirmationPanel != null)
        {
            confirmationPanel.SetActive(false);
        }

        if (customizerPanel != null)
        {
            customizerPanel.SetActive(false);
        }

        if (dummyCharacter != null)
        {
            dummyCharacter.SetActive(false);
        }
    }

    private bool isOverwriting = false;

    public void StartNewGame()
    {
        Debug.Log("StartNewGame elindult! Fõmenü kikapcsolása...");

        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(false);
            Debug.Log("Fõmenü panel kikapcsolva!");
        }
        else
        {
            Debug.LogError("HIBA: A Main Menu Panel nincs behúzva az Inspectorban!");
        }

        if (mainCharacter != null)
        {
            mainCharacter.SetActive(false);
        }

        int emptySlot = SaveSystem.GetFirstEmptySlot(3);

        if (emptySlot != -1)
        {
            isOverwriting = false;
            SaveSystem.SlotToSaveAfterLoad = emptySlot;

            if (warningText != null)
            {
                warningText.text = "There is no save here (Slot " + emptySlot + "). Do you want to start a new game here?";
            }

            if (confirmationPanel != null)
            {
                confirmationPanel.SetActive(true);
            }
        }
        else
        {
            string oldest = SaveSystem.GetOldestSaveFileName();

            if (oldest != null)
            {
                isOverwriting = true;
                if (warningText != null)
                {
                    warningText.text = "All slots are full! The oldest save (" + oldest + ") will be deleted. Continue?";
                }

                if (confirmationPanel != null)
                {
                    confirmationPanel.SetActive(true);
                }
            }
            else
            {
                OpenCustomizer();
            }
        }
    }

    public void ConfirmNewGame()
    {
        if (confirmationPanel != null)
        {
            confirmationPanel.SetActive(false);
        }

        if (isOverwriting)
        {
            string oldestFile = SaveSystem.GetOldestSaveFileName();

            if (oldestFile != null)
            {
                string slotString = oldestFile.Replace("save_", "");
                slotString = slotString.Replace(".json", "");

                SaveSystem.SlotToSaveAfterLoad = int.Parse(slotString);
                SaveSystem.DeleteOldestSaveFile();
            }
        }

        OpenCustomizer();
    }

    public void OpenCustomizer()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (confirmationPanel != null) confirmationPanel.SetActive(false);
        if (mainCharacter != null) mainCharacter.SetActive(false);
        if (dummyCharacter != null) dummyCharacter.SetActive(true);
        if (customizerPanel != null) customizerPanel.SetActive(true);
    }

    public void CancelNewGame()
    {
        if (confirmationPanel != null) confirmationPanel.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (mainCharacter != null) mainCharacter.SetActive(true);
        if (dummyCharacter != null) dummyCharacter.SetActive(false);
    }

    public void ExecuteFreshStart()
    {
        SaveSystem.ResetAllData();

        if (KeyInventory.Instance != null)
        {
            KeyInventory.Instance.ResetKeys();
        }

        if (UiManager.Instance != null)
        {
            UiManager.Instance.ClearAllKeysFromUI();
        }

        if (BoxInventory.Instance != null)
        {
            BoxInventory.Instance.ClearInventory();
        }

        DialogueManager.SetInkMemory(new System.Collections.Generic.Dictionary<string, string>());

        AsyncLoader.TargetSceneIndex = 2;
        SceneManager.LoadScene(1);
    }

    public void OpenLoadMenu()
    {
        if (timeMenuPanel != null)
        {
            timeMenuPanel.SetActive(false);
        }

        RefreshSlotTexts(loadSlotTexts);

        loadMenuPanel.SetActive(true);
    }

    public void ShowTimeMenu()
    {
        if (loadMenuPanel != null)
        {
            loadMenuPanel.SetActive(false);
        }

        timeMenuPanel.SetActive(true);
    }

    public void CloseLoadMenu()
    {
        loadMenuPanel.SetActive(false);
    }

    public void CloseTimeMenu()
    {
        timeMenuPanel.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void LoadGameFromSlot(int slotIndex)
    {
        SaveSystem.Load(slotIndex);
        AsyncLoader.IsLoadingFromSave = true;
        AsyncLoader.TargetSceneIndex = SaveSystem.CurrentSceneIndex;
        SceneManager.LoadScene(1);
    }

    private void RefreshSlotTexts(TextMeshProUGUI[] slots)
    {
        if (slots == null)
        {
            return;
        }

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null)
            {
                int slotNum = i + 1;
                string path = SaveSystem.SaveFileName(slotNum);

                if (File.Exists(path))
                {
                    System.DateTime date = File.GetLastWriteTime(path);
                    slots[i].text = "SLOT " + slotNum + "\n<size=60%>" + date.ToString("yyyy.MM.dd HH:mm") + "</size>";
                }
                else
                {
                    slots[i].text = "SLOT " + slotNum + "\n<size=60%>Empty</size>";
                }
            }
        }
    }
}
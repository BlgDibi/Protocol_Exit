using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    [Header("Parent Panel")]
    public GameObject pauseMenuParent;

    [Header("Child Panels")]
    public GameObject buttonsPanel;
    public GameObject savePanel;
    public GameObject loadPanel;

    [Header("Slot Texts")]
    public TextMeshProUGUI[] saveSlotTexts;
    public TextMeshProUGUI[] loadSlotTexts;

    [Header("Others")]
    public GameObject crosshair;

    public GameObject[] canvasesToHide;
    public MonoBehaviour[] scriptsToDisable;

    [Header("Background Image")]
    public GameObject background;

    private bool[] previousCanvasStates;
    private bool previousCrosshairState;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (savePanel.activeSelf)
            {
                BackToPause();
            }
            else if (loadPanel.activeSelf)
            {
                BackToPause();
            }
            else if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        UiManager.isMenuOpen = false;

        if (background != null)
        {
            background.SetActive(false);
        }

        pauseMenuParent.SetActive(false);
        savePanel.SetActive(false);
        loadPanel.SetActive(false);

        Time.timeScale = 1f;
        GameIsPaused = false;
        ToggleScripts(true);

        if (crosshair != null)
        {
            crosshair.SetActive(previousCrosshairState);
        }

        if (previousCanvasStates != null)
        {
            for (int i = 0; i < canvasesToHide.Length; i++)
            {
                if (canvasesToHide[i] != null)
                {
                    canvasesToHide[i].SetActive(previousCanvasStates[i]);
                }
            }
        }

        if (DialogueManager.Instance != null)
        {
            if (DialogueManager.Instance.isDialoguePlaying)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Pause()
    {
        UiManager.isMenuOpen = true;

        pauseMenuParent.SetActive(true);
        buttonsPanel.SetActive(true);
        savePanel.SetActive(false);
        loadPanel.SetActive(false);

        if (background != null)
        {
            background.SetActive(true);
        }

        RefreshSlotTexts(saveSlotTexts);
        RefreshSlotTexts(loadSlotTexts);

        Time.timeScale = 0f;
        GameIsPaused = true;
        ToggleScripts(false);

        if (crosshair != null)
        {
            previousCrosshairState = crosshair.activeSelf;
            crosshair.SetActive(false);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        previousCanvasStates = new bool[canvasesToHide.Length];

        for (int i = 0; i < canvasesToHide.Length; i++)
        {
            if (canvasesToHide[i] != null)
            {
                previousCanvasStates[i] = canvasesToHide[i].activeSelf;
                canvasesToHide[i].SetActive(false);
            }
        }
    }

    public void OpenSaveMenu()
    {
        buttonsPanel.SetActive(false);
        RefreshSlotTexts(saveSlotTexts);
        savePanel.SetActive(true);
    }

    public void OpenLoadMenu()
    {
        buttonsPanel.SetActive(false);
        RefreshSlotTexts(loadSlotTexts);
        loadPanel.SetActive(true);
    }

    public void BackToPause()
    {
        savePanel.SetActive(false);
        loadPanel.SetActive(false);

        pauseMenuParent.SetActive(true);
        buttonsPanel.SetActive(true);
    }

    public void SaveSlot(int slotIndex)
    {
        SaveSystem.Save(slotIndex);
        RefreshSlotTexts(saveSlotTexts);
    }

    public void LoadSlot(int slotIndex)
    {
        Time.timeScale = 1f;
        GameIsPaused = false;

        SaveSystem.Load(slotIndex);

        AsyncLoader.IsLoadingFromSave = true;
        AsyncLoader.TargetSceneIndex = SaveSystem.CurrentSceneIndex;

        SceneManager.LoadScene(1);
    }

    public void LoadMenuScene()
    {
        Time.timeScale = 1f;
        SaveSystem.ResetAllData();
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void ToggleScripts(bool state)
    {
        for (int i = 0; i < scriptsToDisable.Length; i++)
        {
            if (scriptsToDisable[i] != null)
            {
                scriptsToDisable[i].enabled = state;
            }
        }
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
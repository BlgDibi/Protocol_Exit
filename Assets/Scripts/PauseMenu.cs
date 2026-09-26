using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    [Header("Parent Panel")]
    public GameObject pauseMenuParent;

    [Header("Child Panels")]
    public GameObject buttonsPanel;
    public GameObject savePanel;
    public GameObject loadPanel;

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
            if (savePanel.activeSelf || loadPanel.activeSelf)
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

        if (background != null) background.SetActive(false);
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

        bool isAnyCustomUIOpen = false;

        if (previousCanvasStates != null)
        {
            for (int i = 0; i < canvasesToHide.Length; i++)
            {
                if (canvasesToHide[i] != null)
                {
                    canvasesToHide[i].SetActive(previousCanvasStates[i]);

                    if (previousCanvasStates[i] == true)
                    {
                        isAnyCustomUIOpen = true;
                    }
                }
            }
        }

        if ((DialogueManager.Instance != null && DialogueManager.Instance.isDialoguePlaying) || isAnyCustomUIOpen)
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

    void Pause()
    {
        UiManager.isMenuOpen = true;

        pauseMenuParent.SetActive(true);
        if (background != null) background.SetActive(true);
        buttonsPanel.SetActive(true);

        savePanel.SetActive(false);
        loadPanel.SetActive(false);

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
        savePanel.SetActive(true);
    }

    public void OpenLoadMenu()
    {
        buttonsPanel.SetActive(false);
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
        Debug.Log("Saved: " + slotIndex);
    }

    public void LoadSlot(int slotIndex)
    {
        Debug.Log("Load From Pause Menu, slot: " + slotIndex);

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
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void ToggleScripts(bool state)
    {
        foreach (MonoBehaviour script in scriptsToDisable)
        {
            if (script != null) script.enabled = state;
        }
    }
}
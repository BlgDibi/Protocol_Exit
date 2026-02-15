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
    public MonoBehaviour[] scriptsToDisable;

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
 
        pauseMenuParent.SetActive(false);
        savePanel.SetActive(false);
        loadPanel.SetActive(false);

        Time.timeScale = 1f;
        GameIsPaused = false;
        ToggleScripts(true);
        if (crosshair != null) crosshair.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Pause()
    {
        pauseMenuParent.SetActive(true); 
        buttonsPanel.SetActive(true);  

        savePanel.SetActive(false);
        loadPanel.SetActive(false);

        Time.timeScale = 0f;
        GameIsPaused = true;
        if (crosshair != null) crosshair.SetActive(false);
        ToggleScripts(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
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
        Debug.Log("Mentve: " + slotIndex);
    }

    public void LoadSlot(int slotIndex)
    {
        SaveSystem.Load(slotIndex);
        Resume();
    }

    public void LoadMenuScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
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
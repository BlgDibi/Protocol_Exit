using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject pausemenuUI;
    public GameObject gameOverUI;
    public GameObject crosshair;

    public MonoBehaviour[] scriptsToDisable;

    private NPCLookPrompt _npcLookPrompt;
    private Interaction _doorInteraction;


    private DialogueManager _dm;

    void Start()
    {
        _npcLookPrompt = FindObjectOfType<NPCLookPrompt>();
        _doorInteraction = FindObjectOfType<Interaction>();

        _dm = FindObjectOfType<DialogueManager>();
    }

    void Update()
    {
        if (gameOverUI.activeInHierarchy) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused) Resume();
            else Pause();
        }
    }

    public void Resume()
    {
        pausemenuUI.SetActive(false);
        GameIsPaused = false;
        Time.timeScale = 1f;

        ToggleScripts(true);

        DialogueManager dm = FindObjectOfType<DialogueManager>();
        if (dm != null && dm.isDialoguePlaying)
        {
      
            dm.ExitDialogueMode();
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Pause()
    {
        pausemenuUI.SetActive(true);
        GameIsPaused = true;

        if (crosshair != null) crosshair.SetActive(false);
        ToggleScripts(false);

        if (_dm != null && _dm.isDialoguePlaying)
        {
            GameObject panel = GameObject.Find("DialoguePanel");
            if (panel != null) panel.SetActive(false);
        }

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void ToggleScripts(bool state)
    {
        if (_npcLookPrompt != null) _npcLookPrompt.enabled = state;
        if (_doorInteraction != null) _doorInteraction.enabled = state;

        foreach (MonoBehaviour script in scriptsToDisable)
        {
            if (script != null) script.enabled = state;
        }
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
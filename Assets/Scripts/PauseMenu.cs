using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject pausemenuUI;
    public GameObject gameOverUI;
    public GameObject crosshair;

    public MonoBehaviour[] scriptsToDisable;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameOverUI != null && gameOverUI.activeInHierarchy)
            {
                Debug.Log("A GameOverUI aktív, ezért nem nyitok menüt.");
                return;
            }

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

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Pause()
    {
        pausemenuUI.SetActive(true);
        GameIsPaused = true;
        Time.timeScale = 0f;

        if (crosshair != null) crosshair.SetActive(false);
        ToggleScripts(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void ToggleScripts(bool state)
    {
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
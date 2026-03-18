using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject timeMenuPanel;

    public void PlayGame()
    {

        SaveSystem.ResetAllData();

        if (KeyInventory.Instance != null)
            KeyInventory.Instance.ResetKeys();

        if (UiManager.Instance != null)
            UiManager.Instance.ClearAllKeysFromUI();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void ShowTimeMenu()
    {
        timeMenuPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject timeMenuPanel;
    public GameObject loadMenuPanel;

    public void StartNewGame()
    {
        SaveSystem.ResetAllData();

        if (KeyInventory.Instance != null)
            KeyInventory.Instance.ResetKeys();

        if (UiManager.Instance != null)
            UiManager.Instance.ClearAllKeysFromUI();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void OpenLoadMenu()
    {
        if (timeMenuPanel != null) timeMenuPanel.SetActive(false);
        loadMenuPanel.SetActive(true);
    }

    public void ShowTimeMenu()
    {
        if (loadMenuPanel != null) loadMenuPanel.SetActive(false);
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
        Debug.Log("Game close...");
        Application.Quit();
    }

   
    public void LoadGameFromSlot(int slotIndex)
    {
        Debug.Log("Trying to load a game from slot: " + slotIndex);

        SaveSystem.Load(slotIndex);
    }
}
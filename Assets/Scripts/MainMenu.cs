using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class MainMenu : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject timeMenuPanel;
    public GameObject loadMenuPanel;
    public GameObject confirmationPanel;
    public GameObject mainMenuPanel;

    [Header("Main Charachter")]
    public GameObject mainCharacter;
    private void Start()
    {
        if (confirmationPanel != null) confirmationPanel.SetActive(false);
    }

    public void StartNewGame()
    {
        // Megnézzük, hogy a 3 slot közül LÉTEZIK-E BÁRMELYIK
        bool hasAnySave = File.Exists(SaveSystem.SaveFileName(1)) ||
                          File.Exists(SaveSystem.SaveFileName(2)) ||
                          File.Exists(SaveSystem.SaveFileName(3));

        if (hasAnySave)
        {
            // Ha bármelyik sloton van mentés, jöhet a figyelmeztetés
            if (confirmationPanel != null) confirmationPanel.SetActive(true);

            if (mainMenuPanel != null) mainMenuPanel.SetActive(false);

            if (mainCharacter != null) mainCharacter.SetActive(false);
        }
        else
        {
            // Ha teljesen üres mind a 3 slot, indul a játék
            ExecuteFreshStart();
        }
    }

    public void ConfirmNewGame()
    {
        if (confirmationPanel != null) confirmationPanel.SetActive(false);

        // Végigmegyünk mind a 3 sloton, és letöröljük, amelyik létezik
        for (int i = 1; i <= 3; i++)
        {
            string path = SaveSystem.SaveFileName(i);
            if (File.Exists(path))
            {
                File.Delete(path);
                Debug.Log($"<color=red>Régi mentésfájl (Slot {i}) törölve!</color>");
            }
        }

        ExecuteFreshStart();
    }

    public void CancelNewGame()
    {
        if (confirmationPanel != null) confirmationPanel.SetActive(false);

        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);

        if (mainCharacter != null) mainCharacter.SetActive(true);
    }

    private void ExecuteFreshStart()
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
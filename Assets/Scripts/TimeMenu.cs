using UnityEngine;
using TMPro;
using System.Diagnostics;
using UnityEngine.Localization;

public class TimeMenu : MonoBehaviour
{
    public TextMeshProUGUI savedTimeText;
    public GameObject timeMenuPanel;

    public LocalizedString gameLineFormat;

    private Stopwatch stopwatch;

    void Start()
    {
        stopwatch = FindFirstObjectByType<Stopwatch>();
        gameLineFormat.StringChanged += UpdateTextOnLanguageChange;
        DisplaySavedTime();
    }

    private void OnDestroy()
    {
        gameLineFormat.StringChanged -= UpdateTextOnLanguageChange;
    }

    private void UpdateTextOnLanguageChange(string value)
    {
        if (timeMenuPanel.activeSelf)
        {
            DisplaySavedTime();
        }
    }

    public void ShowTimeMenu()
    {
        timeMenuPanel.SetActive(true);
        DisplaySavedTime();
    }

    public void HideTimeMenu()
    {
        timeMenuPanel.SetActive(false);
    }

    private void DisplaySavedTime()
    {
        int gameCount = PlayerPrefs.GetInt("GameCount", 0);
        string finalList = "";
        int startIndex = Mathf.Max(0, gameCount - 5);

       
        string linePattern = gameLineFormat.GetLocalizedString();

        for (int i = startIndex; i < gameCount; i++)
        {
            float savedTime = PlayerPrefs.GetFloat("SavedTime" + i, 0f);

            int minutes = Mathf.FloorToInt(savedTime / 60);
            int seconds = Mathf.FloorToInt(savedTime % 60);
            int milliseconds = Mathf.FloorToInt((savedTime * 100) % 100);

            string timeString = $"{minutes:00}:{seconds:00}:{milliseconds:00}";

          
            finalList += string.Format(linePattern, i + 1, timeString) + "\n";
        }

        savedTimeText.text = finalList;
    }

    public void BackToMainMenu()
    {
        if (stopwatch != null)
        {
            stopwatch.SaveTimeOnMainMenu();
        }
        timeMenuPanel.SetActive(false);
    }
}
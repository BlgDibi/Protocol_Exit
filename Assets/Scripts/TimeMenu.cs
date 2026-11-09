using UnityEngine;
using TMPro;
using UnityEngine.Localization;

public class TimeMenu : MonoBehaviour
{
    public TextMeshProUGUI savedTimeText;
    public GameObject timeMenuPanel;

    public LocalizedString gameLineFormat;

    [Header("Rank limits in seconds")]
    public float rankS_MaxTime = 1200f;
    public float rankA_MaxTime = 1500f;
    public float rankB_MaxTime = 1800f;

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
            string savedDate = PlayerPrefs.GetString("SavedDate" + i, "Unknown date");

            int isCompleted = PlayerPrefs.GetInt("GameCompleted" + i, 0);

            int minutes = Mathf.FloorToInt(savedTime / 60);
            int seconds = Mathf.FloorToInt(savedTime % 60);
            int milliseconds = Mathf.FloorToInt((savedTime * 100) % 100);

            string timeString = "";

            if (isCompleted == 1)
            {
                string rank = "";
                if (savedTime <= rankS_MaxTime) rank = "<color=#FFD700>S</color>";
                else if (savedTime <= rankA_MaxTime) rank = "<color=#C0C0C0>A</color>";
                else if (savedTime <= rankB_MaxTime) rank = "<color=#CD7F32>B</color>";
                else rank = "<color=red>C</color>";

                timeString = $"{minutes:00}:{seconds:00}:{milliseconds:00}\t\t  Rank: {rank}";
            }
            else
            {
                timeString = $"{minutes:00}:{seconds:00}:{milliseconds:00} \t\t [<color=black>Game not completed</color>]";
            }

            finalList += string.Format(linePattern, savedDate, timeString) + "\n";
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
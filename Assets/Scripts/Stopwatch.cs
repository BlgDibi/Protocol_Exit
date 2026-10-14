using UnityEngine;
using TMPro;
using UnityEngine.Localization;

public class Stopwatch : MonoBehaviour
{
    public TextMeshProUGUI stopwatchText;
    public LocalizedString timeFormat;

    private float elapsedTime = 0f;
    private bool isRunning = false;
    private string cachedPattern;

    void Start()
    {
        cachedPattern = timeFormat.GetLocalizedString();
        timeFormat.StringChanged += UpdatePattern;

        if (SaveSystem.IsGameJustLoaded)
        {
            elapsedTime = SaveSystem.GetSavedTime();
        }

        UpdateStopwatchUI();
        StartStopwatch();
    }

    void OnDestroy()
    {
        timeFormat.StringChanged -= UpdatePattern;
    }

    private void UpdatePattern(string newString)
    {
        cachedPattern = newString;
        UpdateStopwatchUI();
    }

    void Update()
    {
        if (isRunning)
        {
            elapsedTime += Time.deltaTime;
            UpdateStopwatchUI();
        }
    }

    private void UpdateStopwatchUI()
    {
        if (stopwatchText == null) return;

        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);
        int milliseconds = Mathf.FloorToInt((elapsedTime * 100) % 100);

        string numbers = $"{minutes:00}:{seconds:00}:{milliseconds:00}";

        if (!string.IsNullOrEmpty(cachedPattern))
        {
            stopwatchText.text = string.Format(cachedPattern, numbers);
        }
        else
        {
            stopwatchText.text = numbers;
        }
    }

    public void StartStopwatch() => isRunning = true;
    public void StopStopwatch() => isRunning = false;
    public void ResetStopwatch() { elapsedTime = 0f; UpdateStopwatchUI(); }

    public float GetElapsedTime() => elapsedTime;

    public void SetTime(float time)
    {
        elapsedTime = time;
        UpdateStopwatchUI();
    }

    public void SaveTimeOnMainMenu()
    {
        int gameCount = PlayerPrefs.GetInt("GameCount", 0);
        PlayerPrefs.SetFloat("SavedTime" + gameCount, elapsedTime);

        string currentDate = System.DateTime.Now.ToString("yyyy.MM.dd HH:mm");
        PlayerPrefs.SetString("SavedDate" + gameCount, currentDate);

        PlayerPrefs.SetInt("GameCount", gameCount + 1);
        PlayerPrefs.Save();
        Debug.Log("Time and Date saved to PlayerPrefs for Menu");
    }
}
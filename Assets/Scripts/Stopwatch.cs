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

    public void StartStopwatch()
    {
        isRunning = true;
    }

    public void StopStopwatch()
    {
        isRunning = false;
    }

    public void ResetStopwatch()
    {
        elapsedTime = 0f;
        UpdateStopwatchUI();
    }

    private void SaveElapsedTime()
    {
        int gameCount = PlayerPrefs.GetInt("GameCount", 0);
        PlayerPrefs.SetFloat("SavedTime" + gameCount, elapsedTime);
        PlayerPrefs.SetInt("GameCount", gameCount + 1);
        PlayerPrefs.Save();
    }

    public void SaveTimeOnMainMenu()
    {
        SaveElapsedTime();
    }

    private void OnApplicationQuit()
    {
        SaveElapsedTime();
    }
}
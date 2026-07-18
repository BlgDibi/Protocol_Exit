using UnityEngine;
using System.IO;
using System;

public class CustomLogger : MonoBehaviour
{
    private string logFilePath;

    void Awake()
    {
        logFilePath = Application.persistentDataPath + "/Tesztelo_Log.txt";

        if (File.Exists(logFilePath))
        {
            File.Delete(logFilePath);
        }

        File.AppendAllText(logFilePath, $"--- Játék elindítva: {DateTime.Now} ---\n\n");
    }

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Error || type == LogType.Exception || type == LogType.Warning)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss");

            string logEntry = $"[{timestamp}] {type}: {logString}\n{stackTrace}\n";

            File.AppendAllText(logFilePath, logEntry);
        }
    }
}
using UnityEngine;
using System.IO;
using System;

public class CustomLogger : MonoBehaviour
{
    private string logFilePath;

    void Awake()
    {
        // A fájl helyének kijelölése
        logFilePath = Application.persistentDataPath + "/Tesztelo_Log.txt";

        // Korábbi indításból maradt log törlése, hogy ne legyen gigantikus a fájl
        if (File.Exists(logFilePath))
        {
            File.Delete(logFilePath);
        }

        // Egy nyitó üzenet beírása
        File.AppendAllText(logFilePath, $"--- Játék elindítva: {DateTime.Now} ---\n\n");
    }

    void OnEnable()
    {
        // Feliratkozás a Unity beépített hibaüzeneteire
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        // Leiratkozás (nagyon fontos, hogy ne omoljon össze a memóriakezelés)
        Application.logMessageReceived -= HandleLog;
    }

    // Ez a függvény fut le automatikusan, ha a Unity valamilyen üzenetet küld
    void HandleLog(string logString, string stackTrace, LogType type)
    {
        // Csak az Error (hiba) és Exception (kivétel/fagyás), Warning típusú üzeneteket mentjük
        if (type == LogType.Error || type == LogType.Exception || type == LogType.Warning)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss");

            // Üzenet formázása: [Idõpont] Típus: Üzenet + Pontos kódsor
            string logEntry = $"[{timestamp}] {type}: {logString}\n{stackTrace}\n";

            // Kiírás a szöveges fájlba
            File.AppendAllText(logFilePath, logEntry);
        }
    }
}
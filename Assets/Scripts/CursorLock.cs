using UnityEngine;
using UnityEngine.SceneManagement;

public class CursorLock : MonoBehaviour
{
    private bool isCursorLocked = true;

    void Start()
    {
        UpdateCursorState();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().name == "Game")
        {
            isCursorLocked = !isCursorLocked;
            UpdateCursorState();
        }
    }

    void UpdateCursorState()
    {
        if (SceneManager.GetActiveScene().name == "MENU")
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (SceneManager.GetActiveScene().name == "Game")
        {
            if (isCursorLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }
}

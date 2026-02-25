using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Ending2 : MonoBehaviour
{
    [Header("Restart Delay")]
    public float restartDelay = 2f;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            triggered = true;
            StartCoroutine(RestartGameSequence());
        }
    }

    private IEnumerator RestartGameSequence()
    {
        yield return new WaitForSeconds(restartDelay);

        ResetAllGameData();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void ResetAllGameData()
    {
        if (KeyInventory.Instance != null)
            KeyInventory.Instance.ResetKeys();

        HP playerHP = FindFirstObjectByType<HP>();
        if (playerHP != null)
        {
            playerHP.Hp = 100;
            playerHP.isDead = false;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

  
        Time.timeScale = 1f;

        
    }
}

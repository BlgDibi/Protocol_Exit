using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class EndTrigger : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject endingCanvas;
    public Image blackScreen;
    public TextMeshProUGUI subtitleText;
    public Image nextRoomPhoto;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip glitchSound;
    public AudioClip Voice;

    [Header("Settings")]
    public float timeBeforeReset = 4f;

    [TextArea(3, 8)]
    public string finalMessage = "PROTOCOL: EXIT.\n\nPhase 1 - Complete.\n Result: Acceptable.\n\nInitializing phase 2...\n\n\n Made by: Dávid Balog";
    private bool isEndingTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isEndingTriggered)
        {
            isEndingTriggered = true;
            StartCoroutine(EndingSequence(other.gameObject));
        }
    }

    private IEnumerator EndingSequence(GameObject player)
    {
        if (nextRoomPhoto != null) nextRoomPhoto.color = new Color(1, 1, 1, 0);

        OwnMovement movement = player.GetComponent<OwnMovement>();
        if (movement != null) movement.enabled = false;

        endingCanvas.SetActive(true);
        subtitleText.text = "";

        if (audioSource != null && glitchSound != null)
        {
            audioSource.PlayOneShot(glitchSound);
        }

        for (int i = 0; i < 6; i++)
        {
            blackScreen.color = new Color(0, 0, 0, Random.Range(0.7f, 1f));

            subtitleText.text = Random.value > 0.5f ? "<color=red>Error 404</color>" : "11111101010...";

            yield return new WaitForSeconds(Random.Range(0.05f, 0.15f));

            blackScreen.color = new Color(0, 0, 0, 0);
            subtitleText.text = "";

            yield return new WaitForSeconds(Random.Range(0.02f, 0.08f));
        }

        blackScreen.color = new Color(0, 0, 0, 1);
        subtitleText.text = "";

        yield return new WaitForSeconds(1.5f);

        if (audioSource != null && Voice != null)
        {
            audioSource.PlayOneShot(Voice);
        }

        string currentText = "";
        for (int i = 0; i < finalMessage.Length; i++)
        {
            currentText += finalMessage[i];
            subtitleText.text = currentText;

            if (Random.value < 0.35f)
            {
                subtitleText.text = currentText + "<color=red>#</color>";
                yield return new WaitForSeconds(Random.Range(0.1f, 0.3f));
                subtitleText.text = currentText;
            }

            yield return new WaitForSeconds(Random.Range(0.02f, 0.09f));
        }

        yield return new WaitForSeconds(timeBeforeReset);

        subtitleText.text = ""; 

        if (audioSource != null && glitchSound != null)
        {
            audioSource.PlayOneShot(glitchSound);
        }

        if (nextRoomPhoto != null)
        {
            for (int i = 0; i < 5; i++)
            {
                nextRoomPhoto.color = new Color(1, 1, 1, Random.Range(0.5f, 1f));
                yield return new WaitForSeconds(Random.Range(0.05f, 0.15f));

                nextRoomPhoto.color = new Color(1, 1, 1, 0);
                yield return new WaitForSeconds(Random.Range(0.02f, 0.08f));
            }

            nextRoomPhoto.color = new Color(1, 1, 1, 1);
            yield return new WaitForSeconds(3.0f);
        }

        SaveSystem.ResetAllData();

        SceneManager.LoadScene(0);
    }
}
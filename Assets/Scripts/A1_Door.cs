using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Localization; 

public class A1_Door : MonoBehaviour
{
    [Header("Transition Settings")]
    [SerializeField] GameObject fadeout;
    [SerializeField] int sceneToLoad = 2;

    [Header("Lock State")]
    private bool isLocked = true; 
    private bool playerInZone = false; 



    [Header("UI Message")]
    [Tooltip("DoormessageCanvas")]
    public GameObject messagePanel;
    [Tooltip("Text")]
    public TextMeshProUGUI messageText;
    public float messageDuration = 3f;

    [Header("Localization")]
    public LocalizedString lockedMessageKey;

    void Start()
    {
        if (messagePanel != null) messagePanel.SetActive(false);
    }

    void Update()
    {
        if (playerInZone)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (isLocked)
                {
                    HandleLockedDoor();
                }
                else
                {
                   
                    StartTransition();
                }
            }
        }
    }

    public void Unlock()
    {
        isLocked = false;
       
    }

   
    void StartTransition()
    {
        playerInZone = false; 

        if (GetComponent<BoxCollider>() != null)
            GetComponent<BoxCollider>().enabled = false;

        if (messagePanel != null) messagePanel.SetActive(false);

        StartCoroutine(TransitionRoutine());
    }

    IEnumerator TransitionRoutine()
    {
        if (fadeout != null) fadeout.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(sceneToLoad);
       
    }


    private void HandleLockedDoor()
    {
       
        if (!lockedMessageKey.IsEmpty)
        {
            ShowTemporaryMessage(lockedMessageKey.GetLocalizedString());
        }
    }

    private void ShowTemporaryMessage(string text)
    {
        if (messagePanel != null && messageText != null)
        {
            messageText.text = text;
            messagePanel.SetActive(true);

            StopAllCoroutines();
            StartCoroutine(HideMessageAfterDelay());
        }
    }

    private IEnumerator HideMessageAfterDelay()
    {
        yield return new WaitForSeconds(messageDuration);
        if (messagePanel != null) messagePanel.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = false;
            if (messagePanel != null) messagePanel.SetActive(false);
        }
    }
}
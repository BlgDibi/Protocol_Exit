using UnityEngine;

public class Talk : MonoBehaviour
{
    [Header("Transforms")]
    public Transform playerTrans;
    public Transform speakerTrans;
    private Quaternion initialRotation;

    [Header("Audio")]
    public AudioSource dialogueSource; 
    public AudioClip defaultTalkClip;

    private void Start()
    {
        if (speakerTrans != null)
            initialRotation = speakerTrans.rotation;


        if (dialogueSource == null)
        {
            dialogueSource = GetComponent<AudioSource>();
        }
    }

    public void PlayVoiceLine(string fileName)
    {
        // Betöltjük a hangot a Resources/Voices mappából
        AudioClip clip = Resources.Load<AudioClip>("Voices/" + fileName);

        if (clip != null && dialogueSource != null)
        {
            dialogueSource.Stop();
            dialogueSource.clip = clip; // Itt rakja be a kód a megfelelő hangot
            dialogueSource.Play();
            Debug.Log("Hang lejátszása: " + fileName);
        }
        else
        {
            if (clip == null) Debug.LogWarning("NEM TALÁLHATÓ FÁJL: Assets/Resources/Voices/" + fileName);
            if (dialogueSource == null) Debug.LogWarning("HIÁNYZIK AZ AUDIO SOURCE AZ NPC-RŐL!");
        }
    }

    public void LookAtPlayer()
    {
        if (speakerTrans != null && playerTrans != null)
        {
            Vector3 targetPosition = new Vector3(playerTrans.position.x, speakerTrans.position.y, playerTrans.position.z);
            speakerTrans.LookAt(targetPosition);
        }
    }

    public void ResetRotation()
    {
        if (speakerTrans != null)
            speakerTrans.rotation = initialRotation;
    }
}
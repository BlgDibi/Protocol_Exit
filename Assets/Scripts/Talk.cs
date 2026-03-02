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

 
    public void PlayVoiceLine(string tagInfo)
    {
      
        if (dialogueSource != null && defaultTalkClip != null)
        {
            dialogueSource.clip = defaultTalkClip;
            dialogueSource.Play();
        }
    }
}
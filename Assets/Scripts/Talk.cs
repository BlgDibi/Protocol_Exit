using System.Collections;
using UnityEngine;

public class Talk : MonoBehaviour
{
    [Header("Transforms")]
    public Transform playerTrans;
    public Transform speakerTrans;
    private Quaternion initialRotation;

    [Header("Audio & Animation")]
    public AudioSource dialogueSource;
    public AudioClip defaultTalkClip;
    public Animator npcAnimator;

    private void Start()
    {
        if (speakerTrans != null)
            initialRotation = speakerTrans.rotation;

        if (dialogueSource == null)
        {
            dialogueSource = GetComponent<AudioSource>();
        }

        if (npcAnimator == null)
        {
            npcAnimator = GetComponent<Animator>();
        }
    }

    public void PlayVoiceLine(string fileName)
    {
        AudioClip clip = Resources.Load<AudioClip>("Voices/" + fileName);

        if (clip != null && dialogueSource != null)
        {
            dialogueSource.Stop();
            dialogueSource.clip = clip;
            dialogueSource.Play();
            Debug.Log("Hang lejátszása: " + fileName);

      
            if (npcAnimator != null)
            {
                StopAllCoroutines();
                StartCoroutine(TalkAnimationRoutine(clip.length));
            }
        }
        else
        {
            if (clip == null) Debug.LogWarning("NEM TALÁLHATÓ FÁJL: Assets/Resources/Voices/" + fileName);
            if (dialogueSource == null) Debug.LogWarning("HIÁNYZIK AZ AUDIO SOURCE AZ NPC-RŐL!");
        }
    }


    private IEnumerator TalkAnimationRoutine(float clipLength)
    {
        npcAnimator.SetBool("isTalking", true);

        yield return new WaitForSeconds(clipLength);

        npcAnimator.SetBool("isTalking", false);
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
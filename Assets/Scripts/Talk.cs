using System.Collections;
using UnityEngine;

public class Talk : MonoBehaviour
{
    [Header("Transforms")]
    public Transform playerTrans;
    public Transform speakerTrans;
    private Quaternion initialRotation;

    [Header("NPC Rotation Settings")]
    [Tooltip("Y Rotation - for Mechanic")]
    public float yRotationOffset = 0f;

    [Header("Audio & Animation")]
    public AudioSource dialogueSource;
    public AudioClip defaultTalkClip;
    public Animator npcAnimator;

    [Header("Item Settings")]
    public GameObject c4Item;
    public float timeToAppear = 1.0f;

    private void Start()
    {
        if (speakerTrans != null)
            initialRotation = speakerTrans.rotation;

        if (dialogueSource == null)
            dialogueSource = GetComponent<AudioSource>();

        if (npcAnimator == null)
            npcAnimator = GetComponent<Animator>();

        if (c4Item != null)
            c4Item.SetActive(false);
    }

    public void PlayVoiceLine(string fileName)
    {
        AudioClip clip = Resources.Load<AudioClip>("Voices/" + fileName);

        if (clip != null && dialogueSource != null)
        {
            dialogueSource.Stop();
            dialogueSource.clip = clip;
            dialogueSource.Play();

            if (npcAnimator != null)
            {
                StopAllCoroutines();
                StartCoroutine(TalkAnimationRoutine(clip.length));

                if (fileName == "mech_intro_2")
                {
                    npcAnimator.SetTrigger("ShowItem");

                    if (c4Item != null)
                        StartCoroutine(ShowItemAfterDelay());
                }
            }
        }
    }

    private IEnumerator ShowItemAfterDelay()
    {
        yield return new WaitForSeconds(timeToAppear);

        if (c4Item != null)
            c4Item.SetActive(true);
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
            StartCoroutine(SmoothTurnToPlayer());
        }
    }

    private IEnumerator SmoothTurnToPlayer()
    {
        Vector3 targetPosition = new Vector3(playerTrans.position.x, speakerTrans.position.y, playerTrans.position.z);
        Quaternion startRot = speakerTrans.rotation;

        Quaternion targetRot = Quaternion.LookRotation(targetPosition - speakerTrans.position);

        targetRot *= Quaternion.Euler(0, yRotationOffset, 0);

        float turnProgress = 0f;
        while (turnProgress < 1f)
        {
            turnProgress += Time.deltaTime * 1.5f;
            speakerTrans.rotation = Quaternion.Slerp(startRot, targetRot, turnProgress);
            yield return null;
        }
    }

    public void ResetRotation()
    {
        if (speakerTrans != null)
            speakerTrans.rotation = initialRotation;
    }

    public void HideItem()
    {
        if (c4Item != null)
            c4Item.SetActive(false);
    }
}
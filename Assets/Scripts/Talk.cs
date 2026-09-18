using System.Collections;
using UnityEngine;

public class Talk : MonoBehaviour
{
    [Header("Audio & Animation")]
    public AudioSource dialogueSource;
    public Animator npcAnimator;

    [Header("Item Settings")]
    public GameObject c4Item;
    public float timeToAppear = 1.0f;
    public float timeToHide = 1.0f;

    private Coroutine talkAnimCoroutine;

    private void Start()
    {
        if (dialogueSource == null) dialogueSource = GetComponent<AudioSource>();
        if (npcAnimator == null) npcAnimator = GetComponent<Animator>();
        if (c4Item != null) c4Item.SetActive(false);
    }

    public void PlayVoiceLine(string fileName)
    {
        if (npcAnimator != null)
        {
            if (fileName == "mech_intro_2")
            {
                npcAnimator.SetTrigger("ShowItem");
                StartCoroutine(ShowItemRoutine(timeToAppear));
            }
            else if (fileName == "mech_bye")
            {
                npcAnimator.SetTrigger("PutAwayItem");
                StartCoroutine(HideItemRoutine(timeToHide));
            }
        }

        AudioClip clip = Resources.Load<AudioClip>("Voices/" + fileName);
        if (clip != null && dialogueSource != null)
        {
            dialogueSource.Stop();
            dialogueSource.clip = clip;
            dialogueSource.Play();

            if (npcAnimator != null)
            {
                if (talkAnimCoroutine != null) StopCoroutine(talkAnimCoroutine);
                talkAnimCoroutine = StartCoroutine(TalkAnimationRoutine(clip.length));
            }
        }
    }

    private IEnumerator ShowItemRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (c4Item != null) c4Item.SetActive(true);
    }

    private IEnumerator HideItemRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (c4Item != null) c4Item.SetActive(false);
    }

    private IEnumerator TalkAnimationRoutine(float clipLength)
    {
        npcAnimator.SetBool("isTalking", true);
        yield return new WaitForSeconds(clipLength);
        npcAnimator.SetBool("isTalking", false);
    }

    public void HideItem()
    {
        if (c4Item != null) c4Item.SetActive(false);
    }

    public void LookAtPlayer() { }
    public void ResetRotation() { }
}
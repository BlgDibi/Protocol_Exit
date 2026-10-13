using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TV_Event : MonoBehaviour
{
    [Header("TV screen")]
    public MeshRenderer tvScreenRenderer;
    public Material ClassroomMaterial;

    [Header("Audio settings")]
    public AudioSource tvAudioSource;
    public AudioClip bellSound;
    public AudioClip VoiceSound;

    [Header("Save State Logic")]
    public Key keyThatTriggersThis;

    private bool hasTriggered = false;

    public bool isEventFinished = false;

    void Start()
    {
        StartCoroutine(CheckStatusAfterLoad());
    }

    private IEnumerator CheckStatusAfterLoad()
    {
        yield return new WaitForSeconds(0.15f);

        if (keyThatTriggersThis != null)
        {
            List<int> savedKeys = SaveSystem.GetHistoricalKeys();

            if (savedKeys != null && savedKeys.Contains(keyThatTriggersThis.id))
            {
                hasTriggered = true;
                isEventFinished = true;

                if (tvScreenRenderer != null && ClassroomMaterial != null)
                {
                    tvScreenRenderer.material = ClassroomMaterial;
                }
            }
        }
    }

    public void TriggerTVEvent()
    {
        if (hasTriggered) return;
        hasTriggered = true;

        StartCoroutine(WaitAndTriggerTV(1f));
    }

    private IEnumerator WaitAndTriggerTV(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (tvScreenRenderer != null && ClassroomMaterial != null)
        {
            tvScreenRenderer.material = ClassroomMaterial;
        }

        StartCoroutine(PlayTVSoundsSequence());
    }

    private IEnumerator PlayTVSoundsSequence()
    {
        if (bellSound != null && tvAudioSource != null)
        {
            tvAudioSource.PlayOneShot(bellSound);
            yield return new WaitForSeconds(bellSound.length);
        }

        if (VoiceSound != null && tvAudioSource != null)
        {
            tvAudioSource.PlayOneShot(VoiceSound);
            yield return new WaitForSeconds(VoiceSound.length);
        }

        isEventFinished = true;
    }
}
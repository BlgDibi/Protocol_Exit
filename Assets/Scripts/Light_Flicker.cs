using System.Collections;
using UnityEngine;

public class Light_Flicker : MonoBehaviour
{
    [Header("Light Settings")]
    public Light targetLight;
    public float minIntensity = 0f;
    public float maxIntensity = 1.5f;

    [Header("Flicker Speed")]
    public float minWaitTime = 0.05f;
    public float maxWaitTime = 0.2f;

    private void Start()
    {
        if (targetLight == null)
        {
            targetLight = GetComponent<Light>();
        }

        StartCoroutine(FlickerRoutine());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator FlickerRoutine()
    {
        while (true)
        {
            targetLight.intensity = Random.Range(minIntensity, maxIntensity);
            float waitTime = Random.Range(minWaitTime, maxWaitTime);
            yield return new WaitForSeconds(waitTime);
        }
    }
}
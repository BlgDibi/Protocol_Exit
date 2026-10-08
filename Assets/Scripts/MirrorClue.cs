using UnityEngine;
using System.Collections;

public class MirrorClue : MonoBehaviour
{
    [Header("Photo in mirror")]
    public GameObject mirrorSecretObject;

    [Header("Required conditions")]
    public string requiredPhotoClue = "SawBedroomClue";
    public string newConditionToUnlockKey = "SawMirrorClue";

    [Header("Player trigger")]
    public float triggerDistance = 3f;
    public Transform player;

    private bool playerDiscoveredSecret = false;

    void Start()
    {
        if (mirrorSecretObject != null)
        {
            mirrorSecretObject.SetActive(false);
        }

        StartCoroutine(CheckStatusAfterLoad());
    }

    private IEnumerator CheckStatusAfterLoad()
    {
        yield return new WaitForSeconds(0.1f);

        if (ConditionManager.Instance != null && ConditionManager.Instance.CheckCondition(newConditionToUnlockKey))
        {
            playerDiscoveredSecret = true;
            if (mirrorSecretObject != null)
            {
                mirrorSecretObject.SetActive(true);
            }
        }
    }

    void Update()
    {
        if (playerDiscoveredSecret || player == null) return;

        if (ConditionManager.Instance != null && ConditionManager.Instance.CheckCondition(requiredPhotoClue))
        {
            float distance = Vector3.Distance(transform.position, player.position);

            if (distance <= triggerDistance)
            {
                playerDiscoveredSecret = true;

                if (mirrorSecretObject != null)
                {
                    mirrorSecretObject.SetActive(true);
                }

                ConditionManager.Instance.SetCondition(newConditionToUnlockKey, true);
            }
            else
            {
                if (mirrorSecretObject != null && mirrorSecretObject.activeSelf)
                {
                    mirrorSecretObject.SetActive(false);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, triggerDistance);
    }
}
using UnityEngine;

public class Steam : MonoBehaviour
{
    [Header("Damage Settings")]
    public float damagePerSecond = 5f;

    [Header("Condition")]
    public string disableCondition = "GasCompleted";

    [Header("Steam visuals")]
    public GameObject steamVisualEffect;

    private bool playerInside = false;
    private HP playerHP;

    private void Start()
    {
        CheckAndDisableSteam();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            playerHP = other.GetComponent<HP>();

            if (playerHP != null)
                playerHP.diedFromSteam = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;

            if (playerHP != null)
            {
                playerHP.diedFromSteam = false;
            }

            playerHP = null;
        }
    }

    private void Update()
    {
        CheckAndDisableSteam();

        if (playerInside && playerHP != null && !playerHP.isDead)
        {
            playerHP.TakeDamage(damagePerSecond * Time.deltaTime);
        }
    }

    private void CheckAndDisableSteam()
    {
        if (!string.IsNullOrEmpty(disableCondition) && ConditionManager.Instance != null)
        {
            if (ConditionManager.Instance.CheckCondition(disableCondition))
            {
                if (playerHP != null) playerHP.diedFromSteam = false;

                if (steamVisualEffect != null && steamVisualEffect.activeSelf)
                {
                    steamVisualEffect.SetActive(false);
                }

                gameObject.SetActive(false);
            }
        }
    }
}
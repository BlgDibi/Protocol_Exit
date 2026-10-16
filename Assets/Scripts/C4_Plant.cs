using System.Collections;
using UnityEngine;

public class C4_Plant : MonoBehaviour
{
    [Header("References")]
    public GameObject c4WorldModel;
    public GameObject detonatorInHand;

    [Header("Barricade / Furniture")]
    public GameObject[] objectsToDestroy;

    [Header("Effects")]
    public ParticleSystem explosionEffect;
    public float explosionDuration = 2.0f;

    [Header("Explosion damage and sound")]
    public AudioClip explosionSound;
    [Tooltip("Radius")]
    public float explosionRadius = 5.0f;
    [Tooltip("Damage")]
    public float explosionDamage = 100f;

    [Header("UI and animation")]
    public GameObject useDetonatorPrompt;
    public Animator characterAnimator;
    public string drawAnimationTrigger = "DrawItem";
    public float drawDelay = 0.3f;

    [Header("Conflict Prevention")]
    public FlashlightController flashlightController;

    [HideInInspector]
    public bool isC4Planted = false;
    private bool isHoldingDetonator = false;
    private bool canExplode = false;
    private bool isExploding = false;

    private IEnumerator Start()
    {
        if (c4WorldModel != null) c4WorldModel.SetActive(false);
        if (detonatorInHand != null) detonatorInHand.SetActive(false);
        if (useDetonatorPrompt != null) useDetonatorPrompt.SetActive(false);

        if (explosionEffect != null) explosionEffect.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.1f);

        CheckAndDestroyBarricade();
    }

    public void CheckAndDestroyBarricade()
    {
        if (ConditionManager.Instance != null && ConditionManager.Instance.CheckCondition("BarricadeDestroyed"))
        {
            foreach (GameObject obj in objectsToDestroy)
            {
                if (obj != null) Destroy(obj);
            }
            Detonator_Pickup.HasDetonator = false;

            this.enabled = false;
        }
    }

    void Update()
    {
        if (isC4Planted && isHoldingDetonator && !isExploding)
        {
            if (!canExplode)
            {
                if (!Input.GetMouseButton(0))
                {
                    canExplode = true;
                }
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                isExploding = true;
                TriggerExplosion();
            }
        }
    }

    public void PlantC4AtSpot(Transform plantPosition)
    {
        if (isC4Planted) return;
        isC4Planted = true;
        canExplode = false;

        if (c4WorldModel != null)
        {
            c4WorldModel.transform.position = plantPosition.position;
            c4WorldModel.transform.rotation = plantPosition.rotation;
            c4WorldModel.SetActive(true);
        }

        C4_Pickup.HasC4 = false;
        if (UiManager.Instance != null) UiManager.Instance.RemoveC4FromUI();

        StartCoroutine(DrawDetonatorRoutine());
    }

    private IEnumerator DrawDetonatorRoutine()
    {
        if (flashlightController != null)
        {
            flashlightController.ForceHideItems();
            flashlightController.enabled = false;
        }

        if (characterAnimator != null)
        {
            characterAnimator.SetTrigger(drawAnimationTrigger);
        }

        yield return new WaitForSeconds(drawDelay);

        if (isC4Planted && detonatorInHand != null)
        {
            detonatorInHand.SetActive(true);
            isHoldingDetonator = true;

            if (useDetonatorPrompt != null)
            {
                useDetonatorPrompt.SetActive(true);
            }
        }
    }

    void TriggerExplosion()
    {
        if (useDetonatorPrompt != null)
            useDetonatorPrompt.SetActive(false);

        if (explosionSound != null)
        {
            AudioSource.PlayClipAtPoint(explosionSound, c4WorldModel != null ? c4WorldModel.transform.position : transform.position);
        }

        Vector3 explosionCenter = c4WorldModel != null ? c4WorldModel.transform.position : transform.position;
        Collider[] hitColliders = Physics.OverlapSphere(explosionCenter, explosionRadius);
        foreach (Collider hit in hitColliders)
        {
            if (hit.CompareTag("Player"))
            {
                HP playerHP = hit.GetComponent<HP>();
                if (playerHP != null && !playerHP.isDead)
                {
                    playerHP.TakeDamage(explosionDamage);
                }
            }
        }

        if (explosionEffect != null)
        {
            explosionEffect.gameObject.SetActive(true);
            explosionEffect.transform.position = explosionCenter;
            explosionEffect.Play();
            StartCoroutine(DisableExplosionRoutine());
        }

        if (c4WorldModel != null) c4WorldModel.SetActive(false);
        if (detonatorInHand != null) detonatorInHand.SetActive(false);

        if (UiManager.Instance != null) UiManager.Instance.RemoveDetonatorFromUI();

        foreach (GameObject obj in objectsToDestroy)
        {
            if (obj != null) Destroy(obj);
        }

        if (ConditionManager.Instance != null)
        {
            ConditionManager.Instance.SetCondition("BarricadeDestroyed", true);
        }

        isHoldingDetonator = false;
        Detonator_Pickup.HasDetonator = false;

        if (flashlightController != null)
        {
            flashlightController.enabled = true;
        }
    }

    private IEnumerator DisableExplosionRoutine()
    {
        yield return new WaitForSeconds(explosionDuration);

        if (explosionEffect != null)
        {
            explosionEffect.gameObject.SetActive(false);
        }

        this.enabled = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if (c4WorldModel != null)
        {
            Gizmos.DrawWireSphere(c4WorldModel.transform.position, explosionRadius);
        }
        else
        {
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }
    }
}
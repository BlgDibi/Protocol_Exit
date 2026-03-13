using StarterAssets;
using System.Collections;
using UnityEngine;

public class HP : MonoBehaviour
{
    [Header("Player Stats")]
    public float Hp = 100f;
    public bool isDead = false;

    public bool diedFromSteam = false;

    [Header("References")]
    public GameOverMenu gameOverMenu;
    public GameObject gravestonePrefab;
    public GameObject playerVisual;
    public float delayBeforeGameOver = 1f;

    private Stopwatch stopwatch;
    private ThirdPersonController tpc;

    private void Start()
    {
        stopwatch = FindFirstObjectByType<Stopwatch>();
        if (stopwatch != null)
            stopwatch.StartStopwatch();

        tpc = GetComponent<ThirdPersonController>();
    }

    public void TakeDamage(float damageAmount)
    {
        if (isDead) return;

        Hp -= damageAmount;
        Hp = Mathf.Max(Hp, 0);

        if (Hp == 0 && !isDead)
        {
            isDead = true;

            if (stopwatch != null)
                stopwatch.StopStopwatch();

            if (tpc != null)
                tpc.isDead = true;

            StartCoroutine(DeathSequence());
        }
    }

    private IEnumerator DeathSequence()
    {
        if (playerVisual != null)
        {
            Renderer[] renderers = playerVisual.GetComponentsInChildren<Renderer>();
            foreach (Renderer r in renderers)
            {
                r.enabled = false;
            }
        }

        if (gravestonePrefab != null)
            Instantiate(gravestonePrefab, transform.position, Quaternion.identity);

        yield return new WaitForSecondsRealtime(delayBeforeGameOver);

        if (gameOverMenu != null)
            gameOverMenu.gameover();
    }
}
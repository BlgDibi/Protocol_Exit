using UnityEngine;

public class Steam : MonoBehaviour
{
    public float damagePerSecond = 5f;
    private bool playerInside = false;
    private HP playerHP;
    private Ending3 ending3;

    private void Start()
    {
        ending3 = FindObjectOfType<Ending3>();
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
            playerHP = null;
        }
    }

    private void Update()
    {
        if (playerInside && playerHP != null && !playerHP.isDead)
        {
            playerHP.TakeDamage(damagePerSecond * Time.deltaTime);
        }
    }
}

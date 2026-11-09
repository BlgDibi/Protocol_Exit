using System.Collections;
using UnityEngine;

public class CrowbarInteraction : MonoBehaviour
{
    [Header("Camera,object")]
    public Transform mainCamera;
    public Animator playerAnimator;
    public GameObject crowbarInHand;

    [Header("Settings")]
    public float interactDistance = 3.0f;
    public float pryDelay = 1.0f;
    public float pryForce = 7.0f;

    [Header("Audio Settings")]
    public AudioSource crowbarAudioSource;
    public AudioClip hitSound;

    private bool isPrying = false;

    void Update()
    {
        if (Input.GetButtonDown("Fire1") == true)
        {
            if (CrowbarPickup.HasCrowbar == false)
            {
                return;
            }

            if (UiManager.isMenuOpen == true)
            {
                return;
            }

            if (crowbarInHand == null || crowbarInHand.activeSelf == false)
            {
                Debug.Log("Crowbar must be in hand!");
                return;
            }

            if (isPrying == true)
            {
                return;
            }

            TryPrying();
        }
    }

    private void TryPrying()
    {
        RaycastHit hit;

        Vector3 origin = mainCamera.transform.position;
        Vector3 direction = mainCamera.transform.forward;

        Debug.DrawRay(origin, direction * interactDistance, Color.red, 2.0f);

        if (Physics.Raycast(origin, direction, out hit, interactDistance) == true)
        {
            if (hit.collider.CompareTag("Pryable") == true)
            {
                StartCoroutine(PryRoutine(hit.collider.gameObject));
            }
        }
    }

    private IEnumerator PryRoutine(GameObject targetPlank)
    {
        isPrying = true;

        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("Pry");
        }

        yield return new WaitForSeconds(pryDelay);

        if (crowbarAudioSource != null && hitSound != null)
        {
            crowbarAudioSource.clip = hitSound;
            crowbarAudioSource.Play();
        }

        Rigidbody plankRb = targetPlank.GetComponent<Rigidbody>();

        if (plankRb != null)
        {
            plankRb.isKinematic = false;
            Vector3 forceDirection = mainCamera.transform.forward + (Vector3.up * 0.5f);
            plankRb.AddForce(forceDirection * pryForce, ForceMode.Impulse);
            plankRb.AddTorque(Random.insideUnitSphere * pryForce, ForceMode.Impulse);
        }

        Plank plankComponent = targetPlank.GetComponent<Plank>();

        if (plankComponent != null)
        {
            plankComponent.OnPried();
        }

        targetPlank.tag = "Untagged";

        yield return new WaitForSeconds(1.0f);

        isPrying = false;
    }
}
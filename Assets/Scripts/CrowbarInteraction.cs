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

    private bool isPrying = false;

    void Update()
    {

        if (Input.GetButtonDown("Fire1"))
        {
            if (!CrowbarPickup.HasCrowbar) return;
            if (UiManager.isMenuOpen) return;

            if (crowbarInHand == null || !crowbarInHand.activeSelf)
            {
                Debug.Log("Crowbar must be in hand!");
                return;
            }

            if (isPrying) return;

            TryPrying();
        }
    }

    private void TryPrying()
    {
        RaycastHit hit;

        Vector3 origin = mainCamera.transform.position;
        Vector3 direction = mainCamera.transform.forward;

        Debug.DrawRay(origin, direction * interactDistance, Color.red, 2.0f);

        if (Physics.Raycast(origin, direction, out hit, interactDistance))
        {

            if (hit.collider.CompareTag("Pryable"))
            {
                StartCoroutine(PryRoutine(hit.collider.gameObject));
            }
            
        }
       
    }

    private IEnumerator PryRoutine(GameObject targetPlank)
    {
        isPrying = true;
        if (playerAnimator != null) playerAnimator.SetTrigger("Pry");

        yield return new WaitForSeconds(pryDelay);

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
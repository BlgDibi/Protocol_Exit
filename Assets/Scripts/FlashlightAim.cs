using UnityEngine;

public class FlashlightAim : MonoBehaviour
{
    [Header("A játékos kamerája")]
    public Camera mainCam;

    void Start()
    {
        if (mainCam == null)
        {
            mainCam = Camera.main;
        }
    }

    void LateUpdate()
    {
        if (mainCam == null) return;

        Ray ray = mainCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out hit, 100f))
        {
            targetPoint = hit.point; 
        }
        else
        {
          
            targetPoint = ray.GetPoint(100f);
        }

        transform.LookAt(targetPoint);
    }
}
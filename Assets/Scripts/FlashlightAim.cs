using UnityEngine;

public class FlashlightAim : MonoBehaviour
{
    [Header("Main camera")]
    public Camera mainCam;

    void Start()
    {
        if (mainCam == null) mainCam = Camera.main;
    }

    void LateUpdate()
    {
        if (mainCam == null) return;

        // A lámpa mindig a kamera nézési irányába fordul
        Vector3 forwardDirection = mainCam.transform.forward;
        transform.LookAt(transform.position + forwardDirection);
    }
}
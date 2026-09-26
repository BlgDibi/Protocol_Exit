using UnityEngine;

public class FlashlightAim : MonoBehaviour
{
    [Header("References")]
    public Camera mainCam;
    public Transform playerBody;

    [Header("Angle Limits")]
    public float maxUpDown = 50f;
    public float maxLeftRight = 50f;

    void Start()
    {
        if (mainCam == null) mainCam = Camera.main;
        if (playerBody == null && transform.parent != null)
            playerBody = transform.parent;
    }

    void LateUpdate()
    {
        if (mainCam == null || playerBody == null) return;

        Quaternion localCamRotation = Quaternion.Inverse(playerBody.rotation) * mainCam.transform.rotation;
        Vector3 cameraAngles = localCamRotation.eulerAngles;

        float currentUpDownAngle;
        if (cameraAngles.x > 180)
        {
            currentUpDownAngle = cameraAngles.x - 360;
        }
        else
        {
            currentUpDownAngle = cameraAngles.x;
        }

        float currentLeftRightAngle;
        if (cameraAngles.y > 180)
        {
            currentLeftRightAngle = cameraAngles.y - 360;
        }
        else
        {
            currentLeftRightAngle = cameraAngles.y;
        }

        currentUpDownAngle = Mathf.Clamp(currentUpDownAngle, -maxUpDown, maxUpDown);
        currentLeftRightAngle = Mathf.Clamp(currentLeftRightAngle, -maxLeftRight, maxLeftRight);

        Quaternion clampedLocalRotation = Quaternion.Euler(currentUpDownAngle, currentLeftRightAngle, 0f);
        transform.rotation = playerBody.rotation * clampedLocalRotation;
    }
}
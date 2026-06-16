using UnityEngine;

public class OwnMovement : MonoBehaviour
{
    [Header("Core Components")]
    public CharacterController controller;
    public Animator animator;

    [Header("Camera Settings")]
    public Transform mainCamera;
    public Transform cameraRoot;
    public float mouseSensitivity = 2f;
    private float cameraXAngle;
    private float cameraYAngle;

    [Header("Movement Settings")]
    public float walkSpeed = 2.0f;
    public float runSpeed = 5.0f;
    public float jumpHeight = 1.2f;
    public float turnSmoothTime = 0.15f;
    private float turnSmoothVelocity;
    public float gravity = -9.81f;
    private Vector3 fallVelocity;

    private float horizontal, vertical;
    private bool isRunning;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        cameraXAngle = cameraRoot.eulerAngles.x;
        cameraYAngle = cameraRoot.eulerAngles.y;
    }

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        isRunning = Input.GetKey(KeyCode.LeftShift);

        if (Input.GetButtonDown("Jump") && controller.isGrounded)
        {
            fallVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetTrigger("Jump");
        }
    }

    void FixedUpdate()
    {
        if (controller == null || animator == null) return;

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
        float targetSpeed = isRunning ? runSpeed : walkSpeed;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + mainCamera.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * targetSpeed * Time.fixedDeltaTime);
            animator.SetFloat("Speed", isRunning ? 1.0f : 0.5f, 0.1f, Time.fixedDeltaTime);
        }
        else
        {
            animator.SetFloat("Speed", 0f, 0.15f, Time.fixedDeltaTime);
        }

        if (controller.isGrounded && fallVelocity.y < 0) fallVelocity.y = -2f;
        fallVelocity.y += gravity * Time.fixedDeltaTime;
        controller.Move(fallVelocity * Time.fixedDeltaTime);
    }

    void LateUpdate()
    {
        if (cameraRoot == null) return;
        cameraYAngle += Input.GetAxis("Mouse X") * mouseSensitivity;
        cameraXAngle -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        cameraXAngle = Mathf.Clamp(cameraXAngle, -30f, 70f);
        cameraRoot.rotation = Quaternion.Euler(cameraXAngle, cameraYAngle, 0f);
    }
}
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

    [Header("Push Settings")]
    public Transform pushPoint;
    public float pushForce = 3000f;
    public float pushDistance = 1.5f;
    public float pushDelay = 0.3f;
    private float pushTimer = 0f;

    [Header("Footstep Audio")]
    public AudioSource footstepAudioSource;
    public AudioClip[] footstepClips;

    private float horizontal, vertical;
    private bool isRunning;
    private bool isPushingObject = false;
    private Rigidbody pushedRigidbody;
    private KeyCode currentPushKey;


    private float lastFootstepTime = 0f;
    public float footstepCooldown = 0.25f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        cameraXAngle = cameraRoot.eulerAngles.x;
        cameraYAngle = cameraRoot.eulerAngles.y;
    }

    void Update()
    {
        bool isLookingAround = Input.GetKey(KeyCode.C);

        if (isLookingAround)
        {
            horizontal = 0f;
            vertical = 0f;
            isRunning = false;
        }
        else
        {
            horizontal = Input.GetAxisRaw("Horizontal");
            vertical = Input.GetAxisRaw("Vertical");
            isRunning = Input.GetKey(KeyCode.LeftShift);
        }

        if (Input.GetButtonDown("Jump") && controller.isGrounded && !isPushingObject)
        {
            fallVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetTrigger("Jump");
        }

        if (isPushingObject)
        {
            if (Input.GetKey(currentPushKey)) ApplyContinuousPush();
            else StopPushing();
        }
    }

    void FixedUpdate()
    {
        if (controller == null || animator == null) return;

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        float targetSpeed;
        if (isRunning == true)
        {
            targetSpeed = runSpeed;
        }
        else
        {
            targetSpeed = walkSpeed;
        }

        if (isPushingObject == true)
        {
            if (pushedRigidbody != null)
            {
                Vector3 boxVelocity = pushedRigidbody.linearVelocity;
                boxVelocity.y = 0;

                if (controller.isGrounded == true && fallVelocity.y < 0)
                {
                    fallVelocity.y = -2f;
                }

                fallVelocity.y += gravity * Time.fixedDeltaTime;
                controller.Move((boxVelocity + fallVelocity) * Time.fixedDeltaTime);
            }
        }
        else
        {
            if (direction.magnitude >= 0.1f)
            {
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + mainCamera.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                controller.Move(moveDir.normalized * targetSpeed * Time.fixedDeltaTime);

                if (isRunning == true)
                {
                    animator.SetFloat("Speed", 1.0f, 0.1f, Time.fixedDeltaTime);
                }
                else
                {
                    animator.SetFloat("Speed", 0.5f, 0.1f, Time.fixedDeltaTime);
                }
            }
            else
            {
                animator.SetFloat("Speed", 0f, 0.15f, Time.fixedDeltaTime);
            }

            if (controller.isGrounded == true && fallVelocity.y < 0)
            {
                fallVelocity.y = -2f;
            }
            fallVelocity.y += gravity * Time.fixedDeltaTime;
            controller.Move(fallVelocity * Time.fixedDeltaTime);
        }
    }

    void LateUpdate()
    {
        if (cameraRoot == null) return;
        cameraYAngle += Input.GetAxis("Mouse X") * mouseSensitivity;
        cameraXAngle -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        cameraXAngle = Mathf.Clamp(cameraXAngle, -50f, 70f);
        cameraRoot.rotation = Quaternion.Euler(cameraXAngle, cameraYAngle, 0f);
    }

    public void PlayFootstepSound()
    {
    
        if (animator.GetFloat("Speed") < 0.1f || !controller.enabled) return;

        if (Time.time - lastFootstepTime < footstepCooldown) return;

        if (footstepAudioSource != null && footstepClips.Length > 0 && controller.isGrounded)
        {
            lastFootstepTime = Time.time;

            int index = Random.Range(0, footstepClips.Length);
            footstepAudioSource.pitch = Random.Range(0.85f, 1.1f);
            footstepAudioSource.PlayOneShot(footstepClips[index]);
        }
    }

    public void StartPushing(Rigidbody rb, KeyCode key)
    {
        if (rb == null || isPushingObject) return;

        isPushingObject = true;
        pushedRigidbody = rb;
        currentPushKey = key;
        pushTimer = 0f;

        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 1f, transform.forward, out hit, 2f))
        {
            if (hit.rigidbody == rb)
            {
                Vector3 flatNormal = new Vector3(hit.normal.x, 0f, hit.normal.z).normalized;
                if (flatNormal != Vector3.zero) transform.rotation = Quaternion.LookRotation(-flatNormal);
            }
        }
        animator.SetBool("isPushing", true);
    }

    private void ApplyContinuousPush()
    {
        if (pushedRigidbody != null)
        {
            pushTimer += Time.deltaTime;
            if (pushTimer >= pushDelay)
            {
                if (vertical > 0.1f)
                {
                    Vector3 pushDirection = transform.forward;
                    pushDirection.y = 0;

                    Vector3 applyPosition;
                    if (pushPoint != null)
                    {
                        applyPosition = pushPoint.position;
                    }
                    else
                    {
                        applyPosition = transform.position + (Vector3.up * 1f);
                    }

                    pushedRigidbody.AddForceAtPosition(pushDirection * pushForce * Time.deltaTime, applyPosition, ForceMode.Force);
                }
            }
        }
    }

    private void StopPushing()
    {
        isPushingObject = false;
        pushedRigidbody = null;
        pushTimer = 0f;
        animator.SetBool("isPushing", false);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Vector3 startPos;
        if (pushPoint != null)
        {
            startPos = pushPoint.position;
        }
        else
        {
            startPos = transform.position + (Vector3.up * 1f);
        }

        Gizmos.DrawRay(startPos, transform.forward * pushDistance);
    }
}
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;

    [Header("Mouse Look")]
    public Transform cameraTransform;
    public float mouseSensitivity = 0.1f;
    public float minPitch = -80f;
    public float maxPitch = 80f;

    private CharacterController controller;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private float verticalVelocity;
    private float pitch;

    [Header("Research Metrics")]
    private bool isTrackingJump;
    private Vector3 jumpStartPosition;
    private float jumpStartTime;
    private float maxJumpHeight;
    private bool wasGrounded;
    private void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        MovePlayer();
        ApplyMouseLook();
        TrackJumpMetrics();
        EvaluateLanding();
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
    }

    public void OnJump()
    {
        if (controller.isGrounded)
        {
            jumpStartPosition = transform.position;
            jumpStartTime = Time.time;
            maxJumpHeight = transform.position.y;
            isTrackingJump = true;
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    private void MovePlayer()
    {
        if (controller.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
        }

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 horizontalMove = transform.right * moveInput.x + transform.forward * moveInput.y;
        controller.Move(horizontalMove * moveSpeed * Time.deltaTime);
        controller.Move(Vector3.up * verticalVelocity * Time.deltaTime);
    }

    private void ApplyMouseLook()
    {
        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        if (cameraTransform != null)
        {
            cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }
    }

    private void TrackJumpMetrics()
    {
        if (!isTrackingJump)
            return;

        if (transform.position.y > maxJumpHeight)
        {
            maxJumpHeight = transform.position.y;
        }
    }

    private void EvaluateLanding()
    {
        bool currentlyGrounded = controller.isGrounded;
        if (!wasGrounded && currentlyGrounded && isTrackingJump)
        {
            float jumpHeightMeasured = maxJumpHeight - jumpStartPosition.y;
            float airTime = Time.time - jumpStartTime;
            float jumpDistance = Vector3.Distance(new Vector3(jumpStartPosition.x, 0, jumpStartPosition.z), new Vector3(transform.position.x, 0, transform.position.z));
            Debug.Log($"Height={jumpHeightMeasured:F2}m | AirTime={airTime:F2}s | Distance={jumpDistance:F2}m");
            isTrackingJump = false;
        }
        wasGrounded = currentlyGrounded;
    }
}
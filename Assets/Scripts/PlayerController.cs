using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;

    private CharacterController controller;
    private Vector2 moveInput;
    private float verticalVelocity;

    [Header("Research Metrics")]
    private bool isTrackingJump;
    private Vector3 jumpStartPosition;
    private float jumpStartTime;
    private float maxJumpHeight;
    private bool wasGrounded;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        MovePlayer();
        TrackJumpMetrics();
        EvaluateLanding();
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
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
        Vector3 horizontalMove = new Vector3(moveInput.x, 0, moveInput.y);
        controller.Move(horizontalMove * moveSpeed * Time.deltaTime);
        controller.Move(Vector3.up * verticalVelocity * Time.deltaTime);
    }

    // This function tracks the maximum height reached during a jump for the research metrics
    private void TrackJumpMetrics()
    {
        if (!isTrackingJump)
            return;
        if (transform.position.y > maxJumpHeight)
        {
            maxJumpHeight = transform.position.y;
            //Debug.Log($"Current Height: {transform.position.y}");
        }
    }

    // This function evaluates landing conditions and logs jump metrics when the player lands after a jump.
    private void EvaluateLanding()
    {
        bool currentlyGrounded = controller.isGrounded;
        if (!wasGrounded && currentlyGrounded && isTrackingJump)
        {
            float jumpHeightMeasured = maxJumpHeight - jumpStartPosition.y;
            float airTime = Time.time - jumpStartTime;
            float jumpDistance = Vector3.Distance(new Vector3(jumpStartPosition.x, 0, jumpStartPosition.z), new Vector3(transform.position.x, 0, transform.position.z));
            Debug.Log($"Height={jumpHeightMeasured:F2}m | " + $"AirTime={airTime:F2}s | " + $"Distance={jumpDistance:F2}m");
            isTrackingJump = false;
        }
        wasGrounded = currentlyGrounded;
    }
}
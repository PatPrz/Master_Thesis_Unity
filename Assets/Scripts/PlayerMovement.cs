using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 8f;
    public float jumpForce = 12f;

    private Rigidbody rb;
    private bool isGrounded;
    private MovingPlatform activePlatform;

    public float normalDrag = 1f;
    public float fallMultiplier = 3f;
    public float lowJumpMultiplier = 2f;
    public float bounceForce = 13f;
    public float climbSpeed = 5f;
    private bool isClimbing = false;

    public Transform respawnPoint;
    public float waterLevel = -2f;
    private bool isOnSlippery;

    private float moveX;
    private float moveZ;
    private bool jumpRequested;

    private int jumpLogsCount = 0;
    private int moveLogsCount = 0;
    private bool isCurrentlyMoving = false;
    private float jumpInputTimestamp;

    void Start() { rb = GetComponent<Rigidbody>(); }

    void Update()
    {
        moveX = Input.GetAxis("Horizontal");
        moveZ = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            jumpRequested = true;
            jumpInputTimestamp = Time.realtimeSinceStartup;
        }

        if (transform.position.y < waterLevel) Respawn();
    }

    void FixedUpdate()
    {
        if (isClimbing) Climb();
        else { Move(); Jump(); }
    }

    void Move()
    {
        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        Vector3 targetVelocity = move * moveSpeed;
        Vector3 finalVelocity = targetVelocity;

        if (activePlatform != null) finalVelocity += activePlatform.CurrentVelocity;
        finalVelocity.y = rb.linearVelocity.y;

        if (isOnSlippery)
        {
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, finalVelocity, 0.08f);
            rb.linearDamping = 0f;
        }
        else
        {
            rb.linearVelocity = finalVelocity;
            rb.linearDamping = normalDrag;
        }

        if (move.magnitude > 0.1f)
        {
            if (!isCurrentlyMoving && moveLogsCount < 10 && EventResearchManager.Instance != null)
            {
                EventResearchManager.Instance.LogEvent("MOVE_" + (moveLogsCount + 1), Time.realtimeSinceStartup);
                moveLogsCount++;
                isCurrentlyMoving = true;
            }
        }
        else
        {
            isCurrentlyMoving = false;
        }
    }

    void Jump()
    {
        if (jumpRequested)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            if (jumpLogsCount < 10 && EventResearchManager.Instance != null)
            {
                EventResearchManager.Instance.LogEvent("JUMP_" + (jumpLogsCount + 1), jumpInputTimestamp);
                jumpLogsCount++;
            }

            jumpRequested = false;
        }

        if (rb.linearVelocity.y < 0) rb.linearVelocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        else if (rb.linearVelocity.y > 0 && !Input.GetKey(KeyCode.Space)) rb.linearVelocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
    }

    private void OnCollisionStay(Collision collision)
    {
        isGrounded = false;
        foreach (ContactPoint contact in collision.contacts)
        {
            if (Vector3.Dot(contact.normal, Vector3.up) > 0.5f) isGrounded = true;
        }
        isOnSlippery = collision.gameObject.CompareTag("Slippery");
        if (collision.gameObject.CompareTag("Platform")) activePlatform = collision.gameObject.GetComponent<MovingPlatform>();
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Platform")) activePlatform = null;
        isGrounded = false;
        isOnSlippery = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bouncy"))
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * bounceForce, ForceMode.Impulse);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ladder")) { isClimbing = true; rb.useGravity = false; rb.linearVelocity = Vector3.zero; }
        if (other.CompareTag("Water")) Respawn();
    }

    private void OnTriggerExit(Collider other) { if (other.CompareTag("Ladder")) { isClimbing = false; rb.useGravity = true; } }

    void Climb() { rb.linearVelocity = new Vector3(0f, moveZ * climbSpeed, 0f); }

    void Respawn() { transform.position = respawnPoint.position; rb.linearVelocity = Vector3.zero; rb.angularVelocity = Vector3.zero; isClimbing = false; rb.useGravity = true; }
}
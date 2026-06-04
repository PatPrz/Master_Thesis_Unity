using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Vector3 moveDirection = Vector3.right;
    public float moveDistance = 3f;
    public float moveSpeed = 2f;

    [HideInInspector] public float activationTime = 0f;
    private static float lastLoggedSessionTime = -1f;

    private Vector3 startPos;
    private Vector3 targetPos;
    private bool movingForward = true;
    private Rigidbody rb;

    public Vector3 CurrentVelocity { get; private set; }

    void Start()
    {
        startPos = transform.position;
        targetPos = startPos + moveDirection.normalized * moveDistance;
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        enabled = false;
    }

    void FixedUpdate()
    {
        if (activationTime > 0)
        {
            if (Time.time != lastLoggedSessionTime)
            {
                lastLoggedSessionTime = Time.time;
                if (EventResearchManager.Instance != null)
                {
                    EventResearchManager.Instance.LogEvent("GROUP_PLATFORMS_START", activationTime);
                }
            }

            activationTime = -1f;
        }

        Vector3 currentTarget = movingForward ? targetPos : startPos;
        Vector3 nextPos = Vector3.MoveTowards(rb.position, currentTarget, moveSpeed * Time.fixedDeltaTime);
        CurrentVelocity = (nextPos - rb.position) / Time.fixedDeltaTime;
        rb.MovePosition(nextPos);

        if (Vector3.Distance(rb.position, currentTarget) < 0.001f)
        {
            movingForward = !movingForward;
            CurrentVelocity = Vector3.zero;
        }
    }
}
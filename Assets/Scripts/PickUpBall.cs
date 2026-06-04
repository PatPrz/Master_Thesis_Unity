using UnityEngine;

public class PickUpBall : MonoBehaviour
{
    public Transform holdPoint;
    public float pickUpRange = 3f;
    public float throwForce = 15f;
    public Camera playerCamera;

    private GameObject heldBall;

    private int pickupLogsCount = 0;
    private float pickupInputTimestamp;

    void Update()
    {
        if (heldBall != null && heldBall.transform.parent != holdPoint)
        {
            heldBall = null;
        }

        if (Input.GetKeyDown(KeyCode.E) && heldBall == null)
        {
            pickupInputTimestamp = Time.realtimeSinceStartup;
            HandlePickup();
        }

        HandleThrow();
    }

    void HandlePickup()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, pickUpRange);
        foreach (Collider col in hitColliders)
        {
            if (col.CompareTag("Ball"))
            {
                heldBall = col.gameObject;
                Rigidbody rb = heldBall.GetComponent<Rigidbody>();
                if (rb != null) rb.isKinematic = true;

                heldBall.transform.position = holdPoint.position;
                heldBall.transform.parent = holdPoint;

                if (pickupLogsCount < 3 && EventResearchManager.Instance != null)
                {
                    EventResearchManager.Instance.LogEvent("BALL_PICKUP_" + (pickupLogsCount + 1), pickupInputTimestamp);
                    pickupLogsCount++;
                }

                break;
            }
        }
    }

    void HandleThrow()
    {
        if (heldBall != null && Input.GetMouseButtonDown(0))
        {
            Rigidbody rb = heldBall.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                heldBall.transform.parent = null;

                if (playerCamera != null)
                {
                    rb.AddForce(playerCamera.transform.forward * throwForce, ForceMode.Impulse);
                }
            }
            heldBall = null;
        }
    }
}
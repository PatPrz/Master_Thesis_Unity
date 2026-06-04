using UnityEngine;

public class BallLogic : MonoBehaviour
{
    private Vector3 spawnPosition;
    private bool isGoalReached = false;
    private Rigidbody rb;

    void Start()
    {
        spawnPosition = transform.position;
        rb = GetComponent<Rigidbody>();
    }

    public void SetGoalReached()
    {
        isGoalReached = true;
        GetComponent<Renderer>().material.color = Color.green;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water") && !isGoalReached)
        {
            Respawn();
        }
    }

    void Respawn()
    {
        if (rb == null) return;

        if (!rb.isKinematic)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        transform.position = spawnPosition;

        transform.parent = null;
        rb.isKinematic = false;
    }
}
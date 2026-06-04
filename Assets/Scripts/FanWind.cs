using UnityEngine;

public class FanWind : MonoBehaviour
{
    [Header("Wind Settings")]
    public Vector3 windDirection = Vector3.up;
    public float windForce = 20f;
    public float windLength = 10f;

    public float windWidth = 2f;
    public float windHeight = 2f;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 start = transform.position;
        Vector3 end = transform.position + windDirection.normalized * windLength;
        Gizmos.DrawLine(start, end);

        Vector3 halfExtents = Vector3.one;

        if (Mathf.Abs(windDirection.x) > 0)
        {
            halfExtents.x = windLength / 2f;
            halfExtents.y = windHeight / 2f;
            halfExtents.z = windWidth / 2f;
        }
        else if (Mathf.Abs(windDirection.y) > 0)
        {
            halfExtents.x = windWidth / 2f;
            halfExtents.y = windLength / 2f;
            halfExtents.z = windHeight / 2f;
        }
        else if (Mathf.Abs(windDirection.z) > 0)
        {
            halfExtents.x = windWidth / 2f;
            halfExtents.y = windHeight / 2f;
            halfExtents.z = windLength / 2f;
        }

        Gizmos.DrawWireCube(transform.position + windDirection.normalized * windLength / 2f, halfExtents * 2f);
    }

    private void FixedUpdate()
    {
        Vector3 halfExtents = Vector3.one;

        if (Mathf.Abs(windDirection.x) > 0)
        {
            halfExtents.x = windLength / 2f;
            halfExtents.y = windHeight / 2f;
            halfExtents.z = windWidth / 2f;
        }
        else if (Mathf.Abs(windDirection.y) > 0)
        {
            halfExtents.x = windWidth / 2f;
            halfExtents.y = windLength / 2f;
            halfExtents.z = windHeight / 2f;
        }
        else if (Mathf.Abs(windDirection.z) > 0)
        {
            halfExtents.x = windWidth / 2f;
            halfExtents.y = windHeight / 2f;
            halfExtents.z = windLength / 2f;
        }

        Collider[] colliders = Physics.OverlapBox(transform.position + windDirection.normalized * windLength / 2f, halfExtents, transform.rotation);
        foreach (Collider col in colliders)
        {
            Rigidbody rb = col.attachedRigidbody;
            if (rb != null && !rb.isKinematic)
            {
                rb.AddForce(windDirection.normalized * windForce, ForceMode.Force);
            }
        }
    }
}
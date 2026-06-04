using UnityEngine;

public class ShieldTrigger : MonoBehaviour
{
    public MovingPlatform[] platforms;
    private bool activated = false;

    private void OnTriggerEnter(Collider other)
    {
        if (activated) return;

        if (other.CompareTag("Ball"))
        {
            activated = true;
            float hitTime = Time.realtimeSinceStartup;

            foreach (var p in platforms)
            {
                if (p != null)
                {
                    p.activationTime = hitTime;
                    p.enabled = true;
                }
            }

            if (GetComponent<MeshRenderer>()) GetComponent<MeshRenderer>().enabled = false;
            if (GetComponent<Collider>()) GetComponent<Collider>().enabled = false;
            Destroy(gameObject, 1f);
        }
    }
}
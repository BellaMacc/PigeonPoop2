using UnityEngine;
public class RigidbodyDebugger : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;

    [Header("Rotation Limits")]
    [SerializeField] private float maxAngularSpeed = 5f;
    [SerializeField] private bool clampRotation = true;

    private void OnEnable()
    {
        Debug.Log("!!! This project uses a rigid body debugger, which is only a temporary solution! Please fix asapm -BM");
    }
    private void FixedUpdate()
    {
        float angularSpeed = rb.angularVelocity.magnitude;

        if (angularSpeed > maxAngularSpeed)
        {
            Debug.LogWarning($"Excessive rotation detected: {angularSpeed:F2}");

            if (clampRotation)
            {
                rb.angularVelocity =
                    Vector3.ClampMagnitude(rb.angularVelocity, maxAngularSpeed);
            }
        }
    }
}

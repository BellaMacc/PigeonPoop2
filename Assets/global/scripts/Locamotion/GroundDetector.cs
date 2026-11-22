using UnityEngine;

public class GroundDetector : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private Transform detectionOrigin;
    [SerializeField] private float detectionRange = 0.5f;
    [SerializeField] private float detectionRadius = 0.3f;
    [SerializeField] private LayerMask groundLayers = ~0;

    [Header("Surface Requirements")]
    [SerializeField] private float maxLandingSlope = 45f;

    public bool TryFindLandingSurface(out RaycastHit hit)
    {
        hit = default;

        if (detectionOrigin == null)
            return false;

        // Search downward for a surface.
        if (!Physics.SphereCast(
            detectionOrigin.position,
            detectionRadius,
            Vector3.down,
            out hit,
            detectionRange,
            groundLayers,
            QueryTriggerInteraction.Ignore))
        {
            return false;
        }

        // Check whether the surface is flat enough to land on.
        float slope = Vector3.Angle(hit.normal, Vector3.up);

        return slope <= maxLandingSlope;
    }

    private void OnDrawGizmosSelected()
    {
        if (detectionOrigin == null)
            return;

        Vector3 origin = detectionOrigin.position;
        Vector3 end = origin + Vector3.down * detectionRange;

        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(origin, detectionRadius);
        Gizmos.DrawWireSphere(end, detectionRadius);
        Gizmos.DrawLine(origin, end);
    }
}

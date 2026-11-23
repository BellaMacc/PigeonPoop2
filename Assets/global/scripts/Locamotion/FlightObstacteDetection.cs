using UnityEngine;

public class FlightObstacteDetection : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private Transform detectionOrigin;
    [SerializeField] private float detectionDistance = 4f;
    [SerializeField] private float detectionRadius = 0.5f;
    [SerializeField] private LayerMask obstacleLayers;

    public bool TryDetectObstacle(
        Transform player,
        out RaycastHit hit)
    {
        hit = default;

        Vector3 origin = detectionOrigin != null
            ? detectionOrigin.position
            : player.position + Vector3.up * 0.5f;

        Vector3 direction = player.forward;
        direction.y = 0f;
        direction.Normalize();

        return Physics.SphereCast(
            origin,
            detectionRadius,
            direction,
            out hit,
            detectionDistance,
            obstacleLayers,
            QueryTriggerInteraction.Ignore
        );
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 origin = detectionOrigin != null
            ? detectionOrigin.position
            : transform.position + Vector3.up * 0.5f;

        Vector3 direction = transform.forward;
        direction.y = 0f;
        direction.Normalize();

        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            origin + direction * detectionDistance,
            detectionRadius
        );

        Gizmos.DrawLine(
            origin,
            origin + direction * detectionDistance
        );
    }
}

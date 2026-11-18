using UnityEngine;
public class PoopRaycast : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform rayOrigin;

    [Header("Raycast Settings")]
    [SerializeField] private float rayDistance = 100f;
    [SerializeField] private LayerMask hitLayers;

    [Header("Ray Angle")]
    [SerializeField] private float horizontalAngle;
    [SerializeField] private float verticalAngle;
    
       

    [Header("Debug Drawing")]
    [SerializeField] private float drawDuration = 1f;
    [SerializeField] private Color rayColor = Color.red;
    [SerializeField] private Color hitColor = Color.green;

    public bool CastDownward(out RaycastHit hit, out bool isItem)
    {
        isItem = false;

        if (rayOrigin == null)
        {
            hit = default;
            return false;
        }

        Quaternion angleOffset = Quaternion.Euler(
            verticalAngle,
            horizontalAngle,
            0f
        );

        Vector3 direction = angleOffset * rayOrigin.forward;

        bool didHit = Physics.Raycast(
            rayOrigin.position,
            direction,
            out hit,
            rayDistance,
            hitLayers,
            QueryTriggerInteraction.Ignore
        );

        if (didHit)
        {
            isItem = hit.collider.CompareTag("Npc");
        }

        Debug.DrawRay(
            rayOrigin.position,
            direction * (didHit ? hit.distance : rayDistance),
            isItem ? hitColor : rayColor,
            drawDuration
        );

        return didHit;
    }
}


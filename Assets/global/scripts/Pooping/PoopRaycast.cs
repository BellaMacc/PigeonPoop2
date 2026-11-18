using UnityEngine;
public class PoopRaycast : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private Transform rayOrigin;
    [SerializeField] private Camera playerCamera;

    [Header("SphereCast Settings")]
    [SerializeField] private float rayDistance = 100f;
    [SerializeField] private float sphereRadius = 0.5f;
    [SerializeField] private LayerMask hitLayers;

    [Header("NPC Detection")]
    [SerializeField] private string npcTag = "NPC";

    [Header("Debug Drawing")]
    [SerializeField] private float drawDuration = 1f;

    public bool CastDownward()
    {
        if (rayOrigin == null || playerCamera == null)
            return false;

        Debug.Log("pooping!");

        // Get the ray passing through the centre of the screen.
        Ray cameraRay = playerCamera.ViewportPointToRay(
            new Vector3(0.5f, 0.5f, 0f)
        );

        // Find a point along the camera ray.
        Vector3 targetPoint =
            cameraRay.GetPoint(rayDistance);

        // Start at the bird's butt and aim towards the reticle.
        Vector3 origin = rayOrigin.position;
        Vector3 direction = (targetPoint - origin).normalized;

        // Cast the sphere.
        bool didHit = Physics.SphereCast(
            origin,
            sphereRadius,
            direction,
            out RaycastHit hit,
            rayDistance,
            hitLayers,
            QueryTriggerInteraction.Ignore
        );

        if (!didHit)
        {
            Debug.DrawRay(
                origin,
                direction * rayDistance,
                Color.red,
                drawDuration
            );

            return false;
        }
        Debug.Log(
                $"Hit: {hit.collider.gameObject.name} | " +
                $"Tag: {hit.collider.tag} | " +
                $"Distance: {hit.distance:F2}"
            );

        

        if (hit.collider.CompareTag(npcTag))
        {
            npcMovement npc =
                hit.collider.GetComponentInParent<npcMovement>();

            if (npc != null)
                npc.gotHit();

            Debug.DrawRay(
                origin,
                direction * hit.distance,
                Color.green,
                drawDuration
            );

            return npc != null;
        }

        Debug.DrawRay(
            origin,
            direction * hit.distance,
            Color.yellow,
            drawDuration
        );

        return false;
    }
}



using System;
using UnityEngine;

public class BoundaryDetector : MonoBehaviour
{
    [Header("Class References")]
    [Header("Raycast Settings")]
    [SerializeField] private Transform rayOrigin;
    [SerializeField] private float rayDistance = 20f;
    [SerializeField] private LayerMask detectionLayers = ~0;

    public static event Action<float, BoundaryType> OnFoundBoundary;
    public static event Action OnLostBoundary;

    public float HitDistance { get; private set; }
    public bool HasDetectedTarget { get; private set; }


    private void Update()
    {
        CheckForBoundary();
    }

    private void CheckForBoundary()
    {
        bool previouslyDetected = HasDetectedTarget;

        HasDetectedTarget = false;
        HitDistance = 0f;

        if (Physics.Raycast(rayOrigin.position, rayOrigin.forward, out RaycastHit hit, rayDistance, detectionLayers))
        {
            ColliderBoundary detectedObject = hit.collider.GetComponent<ColliderBoundary>();

            if (detectedObject != null)
            {
                HasDetectedTarget = true;
                HitDistance = hit.distance;

                BoundaryType type = detectedObject.GetBoundaryType();
                OnFoundBoundary?.Invoke(HitDistance, type);

                Debug.Log($"Found target at {HitDistance:F2} units away.");
            }
        }

        if (previouslyDetected && !HasDetectedTarget)
        {
            OnLostBoundary?.Invoke();
        }
    }



    private void OnDrawGizmos()
    {
        if (rayOrigin == null) return;

        Gizmos.color = HasDetectedTarget ? Color.green : Color.red;
        Gizmos.DrawRay(rayOrigin.position, rayOrigin.forward * rayDistance);
    }
}

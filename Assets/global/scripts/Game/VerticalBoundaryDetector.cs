using System;
using UnityEngine;

public class VerticalBoundaryDetector : MonoBehaviour
{
    [Header("References")]
    [SerializeField] CharacterController characterController;

    [Header("Vertical Data")]
    [SerializeField] float heightMax;
    [SerializeField] float heightMin;
    [SerializeField] private float currentHeight;

    [Header("Warning Settings")]
    [SerializeField] private float warningDistance = 10f;

    public static event Action<float, BoundaryType> OnHeightLimitDetected;
    public static event Action OnHeightLimitLost;

    public float CurrentHeight => currentHeight;
    public bool HasDetectedLimit { get; private set; }
   
    private void Update()
    {
        CheckHeight();
    }

    private void CheckHeight()
    {
        currentHeight = characterController.transform.position.y;

        float distanceToMax = heightMax- currentHeight;
        float distanceToMin = currentHeight - heightMin;

        float closestDistance = Mathf.Min(distanceToMax, distanceToMin);

        BoundaryType limitType = distanceToMax < distanceToMin? BoundaryType.Sky: BoundaryType.City;

        if (closestDistance <= warningDistance && closestDistance >= 0f)
        {
            HasDetectedLimit = true;
            OnHeightLimitDetected?.Invoke(closestDistance, limitType);
        }
        else
        {
            if (HasDetectedLimit)
            {
                HasDetectedLimit = false;
                OnHeightLimitLost?.Invoke();
            }
        }
    }

}

using UnityEngine;

public class BasicFoodObject : MonoBehaviour
{
    [Header("Food Settings")]
    [SerializeField] private string foodName = "Default";
    [SerializeField] private int refillValue = 1;

    [Header("Ground Placement")]
    [SerializeField] private float groundCheckDistance = 10f;
    [SerializeField] private float groundOffset = 0.1f;
    [SerializeField] private LayerMask groundLayers = ~0;

    private void Start()
    {
        PlaceOnGround();
    }

    private void PlaceOnGround()
    {
        Vector3 origin = transform.position + Vector3.up * 0.5f;

        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, groundCheckDistance, groundLayers))
        {
            transform.position = hit.point + Vector3.up * groundOffset;
        }
    }

    public int GetRefillValue()
    {
        return refillValue;
    }

    public void Consume()
    {
        Destroy(gameObject);
    }
}

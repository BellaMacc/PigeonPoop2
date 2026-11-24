using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ColliderBoundary : MonoBehaviour
{
    [SerializeField] private Collider colliderObj;

    [SerializeField] private BoundaryType boundaryType;
    private void Start()
    {
        colliderObj = GetComponent<Collider>();
    }

    public BoundaryType GetBoundaryType()
    {
        return boundaryType;
    }



}
